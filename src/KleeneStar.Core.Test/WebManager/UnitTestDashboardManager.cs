using KleeneStar.Core.Test;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.Test.WebManager
{
    /// <summary>
    /// Provides unit tests for <see cref="KleeneStar.Core.WebManager.DashboardManager"/>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestDashboardManager
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
        /// Verifies that <c>Add</c> persists the dashboard and that <c>GetDashboard</c>
        /// retrieves it by its business id.
        /// </summary>
        [Fact]
        public void Add_Then_GetDashboard_RoundTrip()
        {
            Seed(nameof(Add_Then_GetDashboard_RoundTrip));

            var dashboard = Sample("Overview");
            CoreHub.DashboardManager.Add(dashboard);

            var loaded = CoreHub.DashboardManager.GetDashboard(dashboard.Id);

            Assert.NotNull(loaded);
            Assert.Equal("Overview", loaded.Name);
        }

        /// <summary>
        /// Verifies that <c>Update</c> writes scalar property changes back to the database.
        /// </summary>
        [Fact]
        public void Update_ChangesScalars()
        {
            Seed(nameof(Update_ChangesScalars));

            var dashboard = Sample("Initial");
            CoreHub.DashboardManager.Add(dashboard);

            dashboard.Name = "Renamed";
            CoreHub.DashboardManager.Update(dashboard);

            var loaded = CoreHub.DashboardManager.GetDashboard(dashboard.Id);
            Assert.NotNull(loaded);
            Assert.Equal("Renamed", loaded.Name);
        }

        /// <summary>
        /// Verifies that <c>Remove</c> deletes the dashboard and raises the
        /// <see cref="KleeneStar.Core.WebManager.IDashboardManager.DashboardRemoved"/> event.
        /// </summary>
        [Fact]
        public void Remove_DeletesAndRaisesEvent()
        {
            Seed(nameof(Remove_DeletesAndRaisesEvent));

            var dashboard = Sample("DeleteMe");
            CoreHub.DashboardManager.Add(dashboard);

            Dashboard raised = null;
            CoreHub.DashboardManager.DashboardRemoved += (_, d) => raised = d;

            CoreHub.DashboardManager.Remove(dashboard.Id);

            Assert.Null(CoreHub.DashboardManager.GetDashboard(dashboard.Id));
            Assert.NotNull(raised);
            Assert.Equal(dashboard.Id, raised.Id);
        }

        /// <summary>
        /// Verifies that <c>GetDashboards(IQuery)</c> returns dashboards from the database.
        /// </summary>
        [Fact]
        public void GetDashboards_ReturnsAllStored()
        {
            Seed(nameof(GetDashboards_ReturnsAllStored));

            CoreHub.DashboardManager.Add(Sample("Alpha"));
            CoreHub.DashboardManager.Add(Sample("Beta"));

            var result = CoreHub.DashboardManager.GetDashboards(new Query<Dashboard>()).ToList();

            Assert.True(result.Count >= 2);
            Assert.Contains(result, d => d.Name == "Alpha");
            Assert.Contains(result, d => d.Name == "Beta");
        }

        /// <summary>
        /// Creates a sample <see cref="Dashboard"/> with a fresh GUID.
        /// </summary>
        /// <param name="name">The dashboard name.</param>
        /// <returns>The sample dashboard.</returns>
        private static Dashboard Sample(string name) => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            State = DashboardState.Active
        };
    }
}
