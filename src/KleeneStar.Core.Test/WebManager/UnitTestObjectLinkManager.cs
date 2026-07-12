using KleeneStar.Core.Test;
using KleeneStar.Model.Entities;
using ObjectEntity = KleeneStar.Model.Entities.Object;

namespace KleeneStar.Core.Test.WebManager
{
    /// <summary>
    /// Provides unit tests for <see cref="KleeneStar.Core.WebManager.ObjectLinkManager"/>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestObjectLinkManager
    {
        private static readonly Guid WorkspaceId = Guid.Parse("88CCDDEE-FF00-1122-3344-55667788AABB");
        private static readonly Guid ClassId = Guid.Parse("99DDEEFF-0011-2233-4455-66778899BBCC");
        private static readonly Guid SourceId = Guid.Parse("AAEEFF00-1122-3344-5566-778899AABBCC");
        private static readonly Guid TargetId = Guid.Parse("BBFF0011-2233-4455-6677-8899AABBCCDD");

        /// <summary>
        /// Seeds the in-memory database with two objects between which links are tested.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        private static void Seed(string connectionString)
        {
            CoreHubFixture.Initialize(connectionString);

            using var db = CoreHubFixture.CreateDbContext(connectionString);

            if (!db.Workspaces.Any(x => x.Id == WorkspaceId))
            {
                db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-link", Name = "main" });
            }
            if (!db.Classes.Any(x => x.Id == ClassId))
            {
                db.Classes.Add(new Class { Id = ClassId, Name = "Incident", WorkspaceId = WorkspaceId });
            }
            if (!db.Objects.Any(x => x.Id == SourceId))
            {
                db.Objects.Add(new ObjectEntity { Id = SourceId, Key = "SRC-1", Summary = "source", WorkspaceId = WorkspaceId, ClassId = ClassId });
            }
            if (!db.Objects.Any(x => x.Id == TargetId))
            {
                db.Objects.Add(new ObjectEntity { Id = TargetId, Key = "TGT-1", Summary = "target", WorkspaceId = WorkspaceId, ClassId = ClassId });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies that <c>Add</c> persists the link and that <c>GetLinks</c>
        /// returns it from either side of the relation.
        /// </summary>
        [Fact]
        public void Add_Then_GetLinks_RoundTrip()
        {
            Seed(nameof(Add_Then_GetLinks_RoundTrip));

            var link = Sample(ObjectLinkRelationType.BlockedBy);
            CoreHub.ObjectLinkManager.Add(link);

            var fromSource = CoreHub.ObjectLinkManager.GetLinks(SourceId).ToList();
            var fromTarget = CoreHub.ObjectLinkManager.GetLinks(TargetId).ToList();

            Assert.Single(fromSource);
            Assert.Single(fromTarget);
            Assert.Equal(link.Id, fromSource[0].Id);
            Assert.Equal(link.Id, fromTarget[0].Id);
        }

        /// <summary>
        /// Verifies that <c>Remove</c> deletes the link and raises the
        /// <see cref="KleeneStar.Core.WebManager.IObjectLinkManager.LinkRemoved"/> event.
        /// </summary>
        [Fact]
        public void Remove_DeletesAndRaisesEvent()
        {
            Seed(nameof(Remove_DeletesAndRaisesEvent));

            var link = Sample(ObjectLinkRelationType.RelatesTo);
            CoreHub.ObjectLinkManager.Add(link);

            ObjectLink? raised = null;
            CoreHub.ObjectLinkManager.LinkRemoved += (_, l) => raised = l;

            CoreHub.ObjectLinkManager.Remove(link);

            Assert.Empty(CoreHub.ObjectLinkManager.GetLinks(SourceId));
            Assert.NotNull(raised);
            Assert.Equal(link.Id, raised.Id);
        }

        /// <summary>
        /// Verifies that <c>GetLinks</c> returns an empty collection when the object
        /// is not part of any link.
        /// </summary>
        [Fact]
        public void GetLinks_Unlinked_ReturnsEmpty()
        {
            Seed(nameof(GetLinks_Unlinked_ReturnsEmpty));

            Assert.Empty(CoreHub.ObjectLinkManager.GetLinks(SourceId));
        }

        /// <summary>
        /// Creates a sample <see cref="ObjectLink"/> from the seeded source to target object.
        /// </summary>
        /// <param name="relation">The relation kind.</param>
        /// <returns>The sample link.</returns>
        private static ObjectLink Sample(ObjectLinkRelationType relation) => new()
        {
            Id = Guid.NewGuid(),
            SourceObjectId = SourceId,
            TargetObjectId = TargetId,
            RelationType = relation
        };
    }
}
