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

            Workspace raised = null;
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
            Assert.Contains("default", KleeneStar.Core.WebManager.WorkspaceManager.ReservedWorkspaceKeys);
            Assert.Contains("admin", KleeneStar.Core.WebManager.WorkspaceManager.ReservedWorkspaceKeys);
            Assert.Contains("api", KleeneStar.Core.WebManager.WorkspaceManager.ReservedWorkspaceKeys);
            Assert.Contains("workspaces", KleeneStar.Core.WebManager.WorkspaceManager.ReservedWorkspaceKeys);
        }

        /// <summary>
        /// Verifies that <c>RecordVisit</c> creates a bookmark that surfaces in
        /// <c>GetRecentWorkspaces</c> and leaves the favorite flag unset.
        /// </summary>
        [Fact]
        public void RecordVisit_CreatesBookmark_AndSurfacesInRecent()
        {
            Seed(nameof(RecordVisit_CreatesBookmark_AndSurfacesInRecent));
            var ownerId = SeedOwner(nameof(RecordVisit_CreatesBookmark_AndSurfacesInRecent));

            var workspace = Sample("alpha");
            CoreHub.WorkspaceManager.Add(workspace);

            var bookmark = CoreHub.WorkspaceManager.RecordVisit(ownerId, workspace.Id);

            Assert.NotNull(bookmark);
            Assert.False(bookmark.Favorite);

            var recent = CoreHub.WorkspaceManager.GetRecentWorkspaces(ownerId, 10);
            Assert.Single(recent);
            Assert.Equal(workspace.Id, recent[0].Id);
            Assert.False(CoreHub.WorkspaceManager.IsFavorite(ownerId, workspace.Id));
        }

        /// <summary>
        /// Verifies that visiting the same workspace twice updates the single bookmark row
        /// in place rather than creating a duplicate (the composite unique index contract).
        /// </summary>
        [Fact]
        public void RecordVisit_Twice_DoesNotDuplicate()
        {
            Seed(nameof(RecordVisit_Twice_DoesNotDuplicate));
            var ownerId = SeedOwner(nameof(RecordVisit_Twice_DoesNotDuplicate));

            var workspace = Sample("alpha");
            CoreHub.WorkspaceManager.Add(workspace);

            CoreHub.WorkspaceManager.RecordVisit(ownerId, workspace.Id);
            CoreHub.WorkspaceManager.RecordVisit(ownerId, workspace.Id);

            using var db = CoreHubFixture.CreateDbContext(nameof(RecordVisit_Twice_DoesNotDuplicate));
            Assert.Equal(1, db.WorkspaceBookmarks.Count(x => x.OwnerId == ownerId && x.WorkspaceId == workspace.Id));
        }

        /// <summary>
        /// Verifies that <c>RecordVisit</c> with an unknown owner or workspace persists nothing
        /// and returns <c>null</c> (the foreign keys would otherwise reject the write).
        /// </summary>
        [Fact]
        public void RecordVisit_UnknownOwnerOrWorkspace_ReturnsNull()
        {
            Seed(nameof(RecordVisit_UnknownOwnerOrWorkspace_ReturnsNull));
            var ownerId = SeedOwner(nameof(RecordVisit_UnknownOwnerOrWorkspace_ReturnsNull));

            var workspace = Sample("alpha");
            CoreHub.WorkspaceManager.Add(workspace);

            Assert.Null(CoreHub.WorkspaceManager.RecordVisit(Guid.NewGuid(), workspace.Id));
            Assert.Null(CoreHub.WorkspaceManager.RecordVisit(ownerId, Guid.NewGuid()));
        }

        /// <summary>
        /// Verifies that <c>SetFavorite</c> toggles the favorite flag and that the workspace
        /// appears in (and disappears from) <c>GetFavoriteWorkspaces</c> accordingly.
        /// </summary>
        [Fact]
        public void SetFavorite_TogglesFlag_AndSurfacesInFavorites()
        {
            Seed(nameof(SetFavorite_TogglesFlag_AndSurfacesInFavorites));
            var ownerId = SeedOwner(nameof(SetFavorite_TogglesFlag_AndSurfacesInFavorites));

            var workspace = Sample("alpha");
            CoreHub.WorkspaceManager.Add(workspace);

            CoreHub.WorkspaceManager.SetFavorite(ownerId, workspace.Id, true);
            Assert.True(CoreHub.WorkspaceManager.IsFavorite(ownerId, workspace.Id));
            Assert.Contains(CoreHub.WorkspaceManager.GetFavoriteWorkspaces(ownerId), w => w.Id == workspace.Id);

            CoreHub.WorkspaceManager.SetFavorite(ownerId, workspace.Id, false);
            Assert.False(CoreHub.WorkspaceManager.IsFavorite(ownerId, workspace.Id));
            Assert.DoesNotContain(CoreHub.WorkspaceManager.GetFavoriteWorkspaces(ownerId), w => w.Id == workspace.Id);
        }

        /// <summary>
        /// Verifies that <c>GetRecentWorkspaces</c> orders by last-visited descending (newest
        /// first) and honours the count cap.
        /// </summary>
        [Fact]
        public void GetRecentWorkspaces_OrdersByLastVisitedDescending()
        {
            Seed(nameof(GetRecentWorkspaces_OrdersByLastVisitedDescending));
            var ownerId = SeedOwner(nameof(GetRecentWorkspaces_OrdersByLastVisitedDescending));

            var older = Sample("older");
            var middle = Sample("middle");
            var newest = Sample("newest");
            CoreHub.WorkspaceManager.Add(older);
            CoreHub.WorkspaceManager.Add(middle);
            CoreHub.WorkspaceManager.Add(newest);

            var now = DateTime.UtcNow;
            SeedBookmark(nameof(GetRecentWorkspaces_OrdersByLastVisitedDescending), ownerId, older.Id, false, now.AddHours(-10));
            SeedBookmark(nameof(GetRecentWorkspaces_OrdersByLastVisitedDescending), ownerId, middle.Id, false, now.AddHours(-5));
            SeedBookmark(nameof(GetRecentWorkspaces_OrdersByLastVisitedDescending), ownerId, newest.Id, false, now.AddHours(-1));

            var recent = CoreHub.WorkspaceManager.GetRecentWorkspaces(ownerId, 10);
            Assert.Equal([newest.Id, middle.Id, older.Id], recent.Select(w => w.Id).ToList());

            var capped = CoreHub.WorkspaceManager.GetRecentWorkspaces(ownerId, 2);
            Assert.Equal([newest.Id, middle.Id], capped.Select(w => w.Id).ToList());
        }

        /// <summary>
        /// Verifies that <c>GetFavoriteWorkspaces</c> returns only active favorites, ordered by
        /// name — archived favorites and non-favorites are excluded.
        /// </summary>
        [Fact]
        public void GetFavoriteWorkspaces_ReturnsOnlyActiveFavorites_OrderedByName()
        {
            Seed(nameof(GetFavoriteWorkspaces_ReturnsOnlyActiveFavorites_OrderedByName));
            var ownerId = SeedOwner(nameof(GetFavoriteWorkspaces_ReturnsOnlyActiveFavorites_OrderedByName));

            var beta = Sample("beta");
            var alpha = Sample("alpha");
            var visitedOnly = Sample("gamma");
            var archived = new Workspace { Id = Guid.NewGuid(), Key = "delta", Name = "delta", State = WorkspaceState.Archived };
            CoreHub.WorkspaceManager.Add(beta);
            CoreHub.WorkspaceManager.Add(alpha);
            CoreHub.WorkspaceManager.Add(visitedOnly);
            CoreHub.WorkspaceManager.Add(archived);

            var now = DateTime.UtcNow;
            SeedBookmark(nameof(GetFavoriteWorkspaces_ReturnsOnlyActiveFavorites_OrderedByName), ownerId, beta.Id, true, now);
            SeedBookmark(nameof(GetFavoriteWorkspaces_ReturnsOnlyActiveFavorites_OrderedByName), ownerId, alpha.Id, true, now);
            SeedBookmark(nameof(GetFavoriteWorkspaces_ReturnsOnlyActiveFavorites_OrderedByName), ownerId, visitedOnly.Id, false, now);
            SeedBookmark(nameof(GetFavoriteWorkspaces_ReturnsOnlyActiveFavorites_OrderedByName), ownerId, archived.Id, true, now);

            var favorites = CoreHub.WorkspaceManager.GetFavoriteWorkspaces(ownerId);

            // alphabetical, active favorites only (alpha, beta) — gamma (not favorite) and delta (archived) excluded
            Assert.Equal([alpha.Id, beta.Id], favorites.Select(w => w.Id).ToList());
        }

        /// <summary>
        /// Seeds an owning identity into the in-memory database and returns its id. The owner must
        /// exist so the bookmark foreign keys accept the write.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        /// <returns>The id of the seeded identity.</returns>
        private static Guid SeedOwner(string connectionString)
        {
            var ownerId = Guid.NewGuid();

            using var db = CoreHubFixture.CreateDbContext(connectionString);
            db.Identities.Add(new Identity
            {
                Id = ownerId,
                Name = "Owner",
                Email = "owner@kleenestar.org",
                PasswordHash = "$test$"
            });
            db.SaveChanges();

            return ownerId;
        }

        /// <summary>
        /// Seeds a single workspace bookmark with explicit favorite and last-visited values so
        /// ordering tests do not depend on wall-clock resolution.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        /// <param name="ownerId">The owning identity id.</param>
        /// <param name="workspaceId">The bookmarked workspace id.</param>
        /// <param name="favorite">The favorite flag.</param>
        /// <param name="lastVisited">The last-visited timestamp.</param>
        private static void SeedBookmark(string connectionString, Guid ownerId, Guid workspaceId, bool favorite, DateTime lastVisited)
        {
            using var db = CoreHubFixture.CreateDbContext(connectionString);
            db.WorkspaceBookmarks.Add(new WorkspaceBookmark
            {
                OwnerId = ownerId,
                WorkspaceId = workspaceId,
                Favorite = favorite,
                LastVisited = lastVisited,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            });
            db.SaveChanges();
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
