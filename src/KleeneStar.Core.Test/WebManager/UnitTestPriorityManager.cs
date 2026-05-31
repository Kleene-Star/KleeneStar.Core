using KleeneStar.Core.Test;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;

namespace KleeneStar.Core.Test.WebManager
{
    /// <summary>
    /// Provides unit tests for <see cref="KleeneStar.Core.WebManager.PriorityManager"/>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestPriorityManager
    {
        private static readonly Guid WorkspaceId = Guid.Parse("CC22334D-4455-6677-8899-AABB00CCDDEE");
        private static readonly Guid ClassId = Guid.Parse("DD33445E-5566-7788-99AA-BBCC11DDEE00");

        /// <summary>
        /// Seeds the in-memory database with a workspace and class for the priorities.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        private static void Seed(string connectionString)
        {
            CoreHubFixture.Initialize(connectionString);

            using var db = CoreHubFixture.CreateDbContext(connectionString);

            if (!db.Workspaces.Any(x => x.Id == WorkspaceId))
            {
                db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-pri", Name = "main" });
            }
            if (!db.Classes.Any(x => x.Id == ClassId))
            {
                db.Classes.Add(new Class { Id = ClassId, Name = "Incident", WorkspaceId = WorkspaceId });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies that <c>Add</c> persists the priority and that <c>GetPriority</c>
        /// retrieves it by its business id.
        /// </summary>
        [Fact]
        public void Add_Then_GetPriority_RoundTrip()
        {
            Seed(nameof(Add_Then_GetPriority_RoundTrip));

            var priority = Sample("High");
            CoreHub.PriorityManager.Add(priority);

            var loaded = CoreHub.PriorityManager.GetPriority(priority.Id);

            Assert.NotNull(loaded);
            Assert.Equal("High", loaded.Name);
        }

        /// <summary>
        /// Verifies that <c>GetPriorities(ClassIdParameter)</c> returns only priorities
        /// attached to the supplied class.
        /// </summary>
        [Fact]
        public void GetPriorities_ByClassId_ReturnsPrioritiesForClass()
        {
            Seed(nameof(GetPriorities_ByClassId_ReturnsPrioritiesForClass));

            CoreHub.PriorityManager.Add(Sample("Low"));
            CoreHub.PriorityManager.Add(Sample("High"));

            var result = CoreHub.PriorityManager.GetPriorities(new ClassIdParameter(ClassId)).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Name == "Low");
            Assert.Contains(result, p => p.Name == "High");
        }

        /// <summary>
        /// Verifies that <c>Update</c> writes scalar property changes back to the database.
        /// </summary>
        [Fact]
        public void Update_ChangesScalars()
        {
            Seed(nameof(Update_ChangesScalars));

            var priority = Sample("Initial");
            CoreHub.PriorityManager.Add(priority);

            priority.Name = "Renamed";
            CoreHub.PriorityManager.Update(priority);

            var loaded = CoreHub.PriorityManager.GetPriority(priority.Id);
            Assert.NotNull(loaded);
            Assert.Equal("Renamed", loaded.Name);
        }

        /// <summary>
        /// Verifies that <c>Remove</c> deletes the priority and raises the
        /// <see cref="KleeneStar.Core.WebManager.IPriorityManager.PriorityRemoved"/> event.
        /// </summary>
        [Fact]
        public void Remove_DeletesAndRaisesEvent()
        {
            Seed(nameof(Remove_DeletesAndRaisesEvent));

            var priority = Sample("DeleteMe");
            CoreHub.PriorityManager.Add(priority);

            Priority? raised = null;
            CoreHub.PriorityManager.PriorityRemoved += (_, p) => raised = p;

            CoreHub.PriorityManager.Remove(priority.Id);

            Assert.Null(CoreHub.PriorityManager.GetPriority(priority.Id));
            Assert.NotNull(raised);
            Assert.Equal(priority.Id, raised.Id);
        }

        /// <summary>
        /// Creates a sample <see cref="Priority"/> attached to the seeded class.
        /// </summary>
        /// <param name="name">The priority name.</param>
        /// <returns>The sample priority.</returns>
        private static Priority Sample(string name) => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            ClassId = ClassId,
            State = PriorityState.Active
        };
    }
}
