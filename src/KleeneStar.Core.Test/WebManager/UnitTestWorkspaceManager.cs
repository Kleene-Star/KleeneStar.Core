using KleeneStar.Core.Test;
using KleeneStar.Model.Entities;

namespace KleeneStar.Core.Test.WebManager
{
    /// <summary>
    /// Provides unit tests for <see cref="KleeneStar.Core.WebManager.WorkspaceManager"/>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestWorkspaceManager
    {
        /// <summary>
        /// Initializes the in-memory database and CoreHub for a single test case.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        private static void Seed(string connectionString)
        {
            CoreHubFixture.Initialize(connectionString);
        }

        /// <summary>
        /// Verifies that <c>Add</c> persists the workspace and that <c>GetWorkspace</c>
        /// retrieves it by its business id.
        /// </summary>
        [Fact]
        public void Add_Then_GetWorkspace_RoundTrip()
        {
            Seed(nameof(Add_Then_GetWorkspace_RoundTrip));

            var workspace = Sample("alpha");
            CoreHub.WorkspaceManager.Add(workspace);

            var loaded = CoreHub.WorkspaceManager.GetWorkspace(workspace.Id);

            Assert.NotNull(loaded);
            Assert.Equal("alpha", loaded.Key);
        }

        /// <summary>
        /// Verifies that <c>GetWorkspaceByKey</c> resolves a workspace by its key
        /// in a case-insensitive way.
        /// </summary>
        [Fact]
        public void GetWorkspaceByKey_IsCaseInsensitive()
        {
            Seed(nameof(GetWorkspaceByKey_IsCaseInsensitive));

            CoreHub.WorkspaceManager.Add(Sample("alpha"));

            Assert.NotNull(CoreHub.WorkspaceManager.GetWorkspaceByKey("alpha"));
            Assert.NotNull(CoreHub.WorkspaceManager.GetWorkspaceByKey("ALPHA"));
            Assert.Null(CoreHub.WorkspaceManager.GetWorkspaceByKey("beta"));
        }

        /// <summary>
        /// Verifies that <c>Update</c> writes scalar property changes back to the database.
        /// </summary>
        [Fact]
        public void Update_ChangesScalars()
        {
            Seed(nameof(Update_ChangesScalars));

            var workspace = Sample("initial");
            CoreHub.WorkspaceManager.Add(workspace);

            workspace.Name = "Renamed";
            CoreHub.WorkspaceManager.Update(workspace);

            var loaded = CoreHub.WorkspaceManager.GetWorkspace(workspace.Id);
            Assert.NotNull(loaded);
            Assert.Equal("Renamed", loaded.Name);
        }

        /// <summary>
        /// Verifies that <c>Remove</c> deletes the workspace and raises the
        /// <see cref="KleeneStar.Core.WebManager.IWorkspaceManager.WorkspaceRemoved"/> event.
        /// </summary>
        [Fact]
        public void Remove_DeletesAndRaisesEvent()
        {
            Seed(nameof(Remove_DeletesAndRaisesEvent));

            var workspace = Sample("delete-me");
            CoreHub.WorkspaceManager.Add(workspace);

            Workspace? raised = null;
            CoreHub.WorkspaceManager.WorkspaceRemoved += (_, w) => raised = w;

            CoreHub.WorkspaceManager.Remove(workspace.Id);

            Assert.Null(CoreHub.WorkspaceManager.GetWorkspace(workspace.Id));
            Assert.NotNull(raised);
            Assert.Equal(workspace.Id, raised.Id);
        }

        /// <summary>
        /// Verifies that <c>Remove</c> is a no-op when the workspace id is unknown.
        /// </summary>
        [Fact]
        public void Remove_Unknown_IsNoOp()
        {
            Seed(nameof(Remove_Unknown_IsNoOp));

            CoreHub.WorkspaceManager.Remove(Guid.NewGuid());
        }

        /// <summary>
        /// Verifies that <c>ReservedWorkspaceKeys</c> blocks well-known URL segments
        /// that would otherwise collide with router endpoints.
        /// </summary>
        [Fact]
        public void ReservedWorkspaceKeys_BlocksRouterSegments()
        {
            Assert.Contains("default",    KleeneStar.Core.WebManager.WorkspaceManager.ReservedWorkspaceKeys);
            Assert.Contains("admin",      KleeneStar.Core.WebManager.WorkspaceManager.ReservedWorkspaceKeys);
            Assert.Contains("api",        KleeneStar.Core.WebManager.WorkspaceManager.ReservedWorkspaceKeys);
            Assert.Contains("workspaces", KleeneStar.Core.WebManager.WorkspaceManager.ReservedWorkspaceKeys);
        }

        /// <summary>
        /// Creates a sample <see cref="Workspace"/> with a fresh GUID and the supplied key.
        /// </summary>
        /// <param name="key">The workspace key.</param>
        /// <returns>The sample workspace.</returns>
        private static Workspace Sample(string key) => new()
        {
            Id = Guid.NewGuid(),
            Key = key,
            Name = key,
            State = WorkspaceState.Active
        };
    }
}
