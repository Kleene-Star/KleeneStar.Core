using KleeneStar.Core.Test;
using KleeneStar.Model.Entities;
using ObjectEntity = KleeneStar.Model.Entities.Object;

namespace KleeneStar.Core.Test.WebManager
{
    /// <summary>
    /// Provides unit tests for <see cref="KleeneStar.Core.WebManager.ObjectManager"/>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestObjectManager
    {
        private static readonly Guid WorkspaceId = Guid.Parse("44778899-AABB-CCDD-EEFF-112233445566");
        private static readonly Guid ClassId = Guid.Parse("55889900-BBCC-DDEE-FF00-223344556677");

        /// <summary>
        /// Seeds the in-memory database with the workspace and class objects attach to.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        private static void Seed(string connectionString)
        {
            CoreHubFixture.Initialize(connectionString);

            using var db = CoreHubFixture.CreateDbContext(connectionString);

            if (!db.Workspaces.Any(x => x.Id == WorkspaceId))
            {
                db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-obj", Name = "main" });
            }
            if (!db.Classes.Any(x => x.Id == ClassId))
            {
                db.Classes.Add(new Class { Id = ClassId, Name = "Incident", WorkspaceId = WorkspaceId });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies that <c>Add</c> persists the object and that <c>GetObject</c>
        /// retrieves it by its business id.
        /// </summary>
        [Fact]
        public void Add_Then_GetObject_RoundTrip()
        {
            Seed(nameof(Add_Then_GetObject_RoundTrip));

            var obj = Sample("INC-1", "Server down");
            CoreHub.ObjectManager.Add(obj);

            var loaded = CoreHub.ObjectManager.GetObject(obj.Id);

            Assert.NotNull(loaded);
            Assert.Equal("Server down", loaded.Summary);
        }

        /// <summary>
        /// Verifies that <c>GetObjectByKey</c> resolves the object by its key.
        /// </summary>
        [Fact]
        public void GetObjectByKey_ReturnsMatch()
        {
            Seed(nameof(GetObjectByKey_ReturnsMatch));

            CoreHub.ObjectManager.Add(Sample("INC-42", "Outage"));

            var loaded = CoreHub.ObjectManager.GetObjectByKey("INC-42");

            Assert.NotNull(loaded);
            Assert.Equal("Outage", loaded.Summary);
        }

        /// <summary>
        /// Verifies that <c>Update</c> writes scalar property changes back to the database.
        /// </summary>
        [Fact]
        public void Update_ChangesScalars()
        {
            Seed(nameof(Update_ChangesScalars));

            var obj = Sample("INC-7", "Initial");
            CoreHub.ObjectManager.Add(obj);

            obj.Summary = "Renamed";
            CoreHub.ObjectManager.Update(obj);

            var loaded = CoreHub.ObjectManager.GetObject(obj.Id);
            Assert.NotNull(loaded);
            Assert.Equal("Renamed", loaded.Summary);
        }

        /// <summary>
        /// Verifies that <c>Remove</c> deletes the object and raises the
        /// <see cref="KleeneStar.Core.WebManager.IObjectManager.ObjectRemoved"/> event.
        /// </summary>
        [Fact]
        public void Remove_DeletesAndRaisesEvent()
        {
            Seed(nameof(Remove_DeletesAndRaisesEvent));

            var obj = Sample("INC-99", "DeleteMe");
            CoreHub.ObjectManager.Add(obj);

            ObjectEntity raised = null;
            CoreHub.ObjectManager.ObjectRemoved += (_, o) => raised = o;

            CoreHub.ObjectManager.Remove(obj.Id);

            Assert.Null(CoreHub.ObjectManager.GetObject(obj.Id));
            Assert.NotNull(raised);
            Assert.Equal(obj.Id, raised.Id);
        }

        /// <summary>
        /// Verifies that <c>GetChildren</c> returns the immediate children of an object
        /// and <c>GetSiblings</c> excludes the reference object itself.
        /// </summary>
        [Fact]
        public void GetChildren_And_GetSiblings_ReturnFamily()
        {
            Seed(nameof(GetChildren_And_GetSiblings_ReturnFamily));

            var parent = Sample("INC-100", "Parent");
            var childA = Sample("INC-101", "ChildA");
            var childB = Sample("INC-102", "ChildB");
            childA.ParentId = parent.Id;
            childB.ParentId = parent.Id;

            CoreHub.ObjectManager.Add(parent);
            CoreHub.ObjectManager.Add(childA);
            CoreHub.ObjectManager.Add(childB);

            var children = CoreHub.ObjectManager.GetChildren(parent.Id).ToList();
            Assert.Equal(2, children.Count);

            var siblings = CoreHub.ObjectManager.GetSiblings(childA.Id).ToList();
            Assert.DoesNotContain(siblings, o => o.Id == childA.Id);
            Assert.Contains(siblings, o => o.Id == childB.Id);
        }

        /// <summary>
        /// Creates a sample object attached to the seeded class and workspace.
        /// </summary>
        /// <param name="key">The object key.</param>
        /// <param name="summary">The object summary.</param>
        /// <returns>The sample object.</returns>
        private static ObjectEntity Sample(string key, string summary) => new()
        {
            Id = Guid.NewGuid(),
            Key = key,
            Summary = summary,
            WorkspaceId = WorkspaceId,
            ClassId = ClassId
        };
    }
}
