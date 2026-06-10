using KleeneStar.Core.Test;
using KleeneStar.Model.Entities;
using ObjectEntity = KleeneStar.Model.Entities.Object;

namespace KleeneStar.Core.Test.WebManager
{
    /// <summary>
    /// Provides unit tests for the hierarchy surface of
    /// <see cref="KleeneStar.Core.WebManager.ObjectManager"/> —
    /// <c>GetAncestors</c>, <c>GetDescendants</c>, and <c>SetParent</c> with its
    /// validation rules (self-parenting, cycles, cross-workspace links, and the
    /// allowed-children declaration of the parent's class).
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestObjectManagerHierarchy
    {
        private static readonly Guid WorkspaceId = Guid.Parse("C52275F8-B051-4F55-EC09-DF728E8FCF21");
        private static readonly Guid OtherWorkspaceId = Guid.Parse("D62386F9-C162-4066-FD1A-E0839F9FD022");
        private static readonly Guid EpicClassId = Guid.Parse("E72497FA-D273-4177-0E2B-F1940A0AE023");
        private static readonly Guid StoryClassId = Guid.Parse("F825A8FB-E384-4288-1F3C-02A51B1BF024");
        private static readonly Guid BugClassId = Guid.Parse("A926B9FC-F495-4399-203D-13B62C2C0025");
        private static readonly Guid RootId = Guid.Parse("BA27CAFD-05A6-44AA-314E-24C73D3D1026");
        private static readonly Guid MidId = Guid.Parse("CB38DBFE-16B7-45BB-425F-35D84E4E2027");
        private static readonly Guid LeafId = Guid.Parse("DC49ECFF-27C8-46CC-5360-46E95F5F3028");
        private static readonly Guid LooseId = Guid.Parse("ED5AFD00-38D9-47DD-6471-57FA60604029");
        private static readonly Guid BugId = Guid.Parse("FE6B0E11-49EA-48EE-7582-680B7171502A");
        private static readonly Guid ForeignId = Guid.Parse("0F7C1F22-5AFB-49FF-8693-791C8282612B");

        /// <summary>
        /// Seeds the in-memory database with a three-level chain (root → mid → leaf)
        /// of Story objects, a loose Story, a Bug, and an object in a second workspace.
        /// The Epic class declares Story as its only allowed child class.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        private static void Seed(string connectionString)
        {
            CoreHubFixture.Initialize(connectionString);

            using var db = CoreHubFixture.CreateDbContext(connectionString);

            if (db.Workspaces.Any(x => x.Id == WorkspaceId))
            {
                return;
            }

            db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-h", Name = "workspace" });
            db.Workspaces.Add(new Workspace { Id = OtherWorkspaceId, Key = "ws-o", Name = "other workspace" });

            var story = new Class { Id = StoryClassId, Name = "Story", WorkspaceId = WorkspaceId };
            var bug = new Class { Id = BugClassId, Name = "Bug", WorkspaceId = WorkspaceId };
            var epic = new Class
            {
                Id = EpicClassId,
                Name = "Epic",
                WorkspaceId = WorkspaceId,
                AllowedChildren = [story]
            };

            db.Classes.Add(story);
            db.Classes.Add(bug);
            db.Classes.Add(epic);

            db.Objects.Add(new ObjectEntity(RootId) { Key = "H-1", Summary = "root", WorkspaceId = WorkspaceId, ClassId = EpicClassId });
            db.Objects.Add(new ObjectEntity(MidId) { Key = "H-2", Summary = "mid", WorkspaceId = WorkspaceId, ClassId = StoryClassId, ParentId = RootId });
            db.Objects.Add(new ObjectEntity(LeafId) { Key = "H-3", Summary = "leaf", WorkspaceId = WorkspaceId, ClassId = StoryClassId, ParentId = MidId });
            db.Objects.Add(new ObjectEntity(LooseId) { Key = "H-4", Summary = "loose", WorkspaceId = WorkspaceId, ClassId = StoryClassId });
            db.Objects.Add(new ObjectEntity(BugId) { Key = "H-5", Summary = "bug", WorkspaceId = WorkspaceId, ClassId = BugClassId });
            db.Objects.Add(new ObjectEntity(ForeignId) { Key = "O-1", Summary = "foreign", WorkspaceId = OtherWorkspaceId, ClassId = StoryClassId });

            db.SaveChanges();
        }

        /// <summary>
        /// GetAncestors walks the chain nearest first (parent before grandparent) and
        /// returns an empty collection for a root object.
        /// </summary>
        [Fact]
        public void GetAncestors_ReturnsChainNearestFirst()
        {
            Seed(nameof(GetAncestors_ReturnsChainNearestFirst));

            var ancestors = CoreHub.ObjectManager.GetAncestors(LeafId).ToList();

            Assert.Equal(2, ancestors.Count);
            Assert.Equal(MidId, ancestors[0].Id);
            Assert.Equal(RootId, ancestors[1].Id);

            Assert.Empty(CoreHub.ObjectManager.GetAncestors(RootId));
        }

        /// <summary>
        /// GetDescendants returns the whole subtree (children and grandchildren) and
        /// excludes the object itself; a leaf has no descendants.
        /// </summary>
        [Fact]
        public void GetDescendants_ReturnsSubtree()
        {
            Seed(nameof(GetDescendants_ReturnsSubtree));

            var descendants = CoreHub.ObjectManager.GetDescendants(RootId).ToList();

            Assert.Equal(2, descendants.Count);
            Assert.Contains(descendants, d => d.Id == MidId);
            Assert.Contains(descendants, d => d.Id == LeafId);
            Assert.DoesNotContain(descendants, d => d.Id == RootId);

            Assert.Empty(CoreHub.ObjectManager.GetDescendants(LeafId));
        }

        /// <summary>
        /// SetParent persists a valid link, stamps the update, and raises
        /// <see cref="KleeneStar.Core.WebManager.IObjectManager.ObjectUpdated"/>.
        /// </summary>
        [Fact]
        public void SetParent_ValidLink_PersistsAndRaises()
        {
            Seed(nameof(SetParent_ValidLink_PersistsAndRaises));

            var raised = 0;
            CoreHub.ObjectManager.ObjectUpdated += (_, _) => raised++;

            var updated = CoreHub.ObjectManager.SetParent(LooseId, RootId);

            Assert.NotNull(updated);
            Assert.Equal(RootId, updated.ParentId);
            Assert.Equal(1, raised);
            Assert.Contains(CoreHub.ObjectManager.GetChildren(RootId), c => c.Id == LooseId);
        }

        /// <summary>
        /// SetParent with <c>null</c> detaches the object from its parent; detaching an
        /// already detached object is a silent no-op without an update event.
        /// </summary>
        [Fact]
        public void SetParent_Null_Detaches()
        {
            Seed(nameof(SetParent_Null_Detaches));

            var updated = CoreHub.ObjectManager.SetParent(LeafId, null);

            Assert.NotNull(updated);
            Assert.Null(updated.ParentId);
            Assert.Empty(CoreHub.ObjectManager.GetAncestors(LeafId));

            var raised = 0;
            CoreHub.ObjectManager.ObjectUpdated += (_, _) => raised++;

            CoreHub.ObjectManager.SetParent(LeafId, null);

            Assert.Equal(0, raised);
        }

        /// <summary>
        /// SetParent returns <c>null</c> when the object does not exist.
        /// </summary>
        [Fact]
        public void SetParent_UnknownObject_ReturnsNull()
        {
            Seed(nameof(SetParent_UnknownObject_ReturnsNull));

            Assert.Null(CoreHub.ObjectManager.SetParent(Guid.NewGuid(), RootId));
        }

        /// <summary>
        /// SetParent rejects self-parenting, unknown parents, and cross-workspace
        /// links.
        /// </summary>
        [Fact]
        public void SetParent_RejectsSelfUnknownAndCrossWorkspace()
        {
            Seed(nameof(SetParent_RejectsSelfUnknownAndCrossWorkspace));

            Assert.Throws<InvalidOperationException>(() => CoreHub.ObjectManager.SetParent(LooseId, LooseId));
            Assert.Throws<InvalidOperationException>(() => CoreHub.ObjectManager.SetParent(LooseId, Guid.NewGuid()));
            Assert.Throws<InvalidOperationException>(() => CoreHub.ObjectManager.SetParent(LooseId, ForeignId));
        }

        /// <summary>
        /// SetParent rejects a parent that is a descendant of the object — the link
        /// would close a cycle (root → mid → leaf → root).
        /// </summary>
        [Fact]
        public void SetParent_RejectsCycle()
        {
            Seed(nameof(SetParent_RejectsCycle));

            Assert.Throws<InvalidOperationException>(() => CoreHub.ObjectManager.SetParent(RootId, LeafId));
        }

        /// <summary>
        /// When the parent's class declares allowed children, only objects of those
        /// classes may nest beneath it; classes without a declaration accept any
        /// child.
        /// </summary>
        [Fact]
        public void SetParent_HonorsAllowedChildren()
        {
            Seed(nameof(SetParent_HonorsAllowedChildren));

            // the Epic class allows only Story children — a Bug must be rejected …
            Assert.Throws<InvalidOperationException>(() => CoreHub.ObjectManager.SetParent(BugId, RootId));

            // … while a Story is accepted.
            var story = CoreHub.ObjectManager.SetParent(LooseId, RootId);
            Assert.Equal(RootId, story!.ParentId);

            // the Story class declares no allowed children — any class may nest.
            var bugUnderStory = CoreHub.ObjectManager.SetParent(BugId, LooseId);
            Assert.Equal(LooseId, bugUnderStory!.ParentId);
        }

        /// <summary>
        /// Re-linking to the current parent is a silent no-op without an update event.
        /// </summary>
        [Fact]
        public void SetParent_SameParent_IsNoOp()
        {
            Seed(nameof(SetParent_SameParent_IsNoOp));

            var raised = 0;
            CoreHub.ObjectManager.ObjectUpdated += (_, _) => raised++;

            var updated = CoreHub.ObjectManager.SetParent(LeafId, MidId);

            Assert.NotNull(updated);
            Assert.Equal(MidId, updated.ParentId);
            Assert.Equal(0, raised);
        }
    }
}
