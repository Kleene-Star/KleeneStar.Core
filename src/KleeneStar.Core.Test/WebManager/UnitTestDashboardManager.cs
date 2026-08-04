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
        /// Verifies that a column-only update renames, resizes, recolors and reorders columns and
        /// that the changes survive a reload, while the widgets of the surviving columns are kept.
        /// </summary>
        [Fact]
        public void SetColumns_RenameReorderRecolor_SurvivesReloadAndKeepsWidgets()
        {
            Seed(nameof(SetColumns_RenameReorderRecolor_SurvivesReloadAndKeepsWidgets));

            var dashboard = SampleWithBoard("Board");
            CoreHub.DashboardManager.Add(dashboard);

            var first = dashboard.Columns[0];
            var second = dashboard.Columns[1];

            // reorder (second first), rename + resize + recolor the first, keep the second as-is
            CoreHub.DashboardManager.SetColumns(dashboard.Id,
            [
                new DashboardColumn(second.Id) { Name = second.Name, Size = second.Size, Color = second.Color },
                new DashboardColumn(first.Id) { Name = "Renamed", Size = "50%", Color = "#123456" }
            ]);

            var loaded = CoreHub.DashboardManager.GetDashboard(dashboard.Id);
            var ordered = loaded.Columns.OrderBy(c => c.Position).ToList();

            Assert.Equal(2, ordered.Count);
            Assert.Equal(second.Id, ordered[0].Id);
            Assert.Equal(first.Id, ordered[1].Id);
            Assert.Equal("Renamed", ordered[1].Name);
            Assert.Equal("50%", ordered[1].Size);
            Assert.Equal("#123456", ordered[1].Color);

            // the widgets of the surviving columns are preserved by a column-only update
            Assert.Equal(2, ordered[0].Widgets.Count + ordered[1].Widgets.Count);
        }

        /// <summary>
        /// Verifies that adding a column (an entry with an empty id) persists a new empty column and
        /// leaves the existing columns and their widgets intact.
        /// </summary>
        [Fact]
        public void SetColumns_Add_PersistsNewColumn()
        {
            Seed(nameof(SetColumns_Add_PersistsNewColumn));

            var dashboard = SampleWithBoard("Board");
            CoreHub.DashboardManager.Add(dashboard);

            var first = dashboard.Columns[0];
            var second = dashboard.Columns[1];

            CoreHub.DashboardManager.SetColumns(dashboard.Id,
            [
                new DashboardColumn(first.Id) { Name = first.Name, Size = "1fr" },
                new DashboardColumn(second.Id) { Name = second.Name, Size = "1fr" },
                new DashboardColumn(Guid.Empty) { Name = "New column", Size = "1fr" }
            ]);

            var loaded = CoreHub.DashboardManager.GetDashboard(dashboard.Id);

            Assert.Equal(3, loaded.Columns.Count);
            Assert.Contains(loaded.Columns, c => c.Name == "New column" && c.Widgets.Count == 0);
        }

        /// <summary>
        /// Verifies that deleting a column (omitting it from the desired set) removes it together with
        /// its widgets.
        /// </summary>
        [Fact]
        public void SetColumns_Delete_RemovesColumnAndWidgets()
        {
            Seed(nameof(SetColumns_Delete_RemovesColumnAndWidgets));

            var dashboard = SampleWithBoard("Board");
            CoreHub.DashboardManager.Add(dashboard);

            var first = dashboard.Columns[0];

            CoreHub.DashboardManager.SetColumns(dashboard.Id,
            [
                new DashboardColumn(first.Id) { Name = first.Name, Size = first.Size }
            ]);

            var loaded = CoreHub.DashboardManager.GetDashboard(dashboard.Id);

            Assert.Single(loaded.Columns);
            Assert.Equal(first.Id, loaded.Columns[0].Id);
        }

        /// <summary>
        /// Verifies that a full board update rebuilds a column's widgets and that the per-widget type,
        /// name, color and params survive a reload.
        /// </summary>
        [Fact]
        public void SetBoard_WidgetSettings_SurviveReload()
        {
            Seed(nameof(SetBoard_WidgetSettings_SurviveReload));

            var dashboard = SampleWithBoard("Board");
            CoreHub.DashboardManager.Add(dashboard);

            var first = dashboard.Columns[0];

            CoreHub.DashboardManager.SetBoard(dashboard.Id,
            [
                new DashboardColumn(first.Id)
                {
                    Name = first.Name,
                    Size = first.Size,
                    Widgets =
                    [
                        new Widget(Guid.NewGuid())
                        {
                            Type = "widget_kleenestar_note",
                            Name = "My Note",
                            Color = "#abcdef",
                            Params = "{\"text\":\"hello\",\"tone\":\"success\"}"
                        }
                    ]
                }
            ]);

            var loaded = CoreHub.DashboardManager.GetDashboard(dashboard.Id);
            var column = loaded.Columns.Single(c => c.Id == first.Id);
            var widget = Assert.Single(column.Widgets);

            Assert.Equal("widget_kleenestar_note", widget.Type);
            Assert.Equal("My Note", widget.Name);
            Assert.Equal("#abcdef", widget.Color);
            Assert.Contains("success", widget.Params);
        }

        /// <summary>
        /// Verifies that a session-new column (identified only by its transient client key) is
        /// correlated across a column update and a later board update, so it is not duplicated, and
        /// that a subsequent column-only update preserves the widget a board update added to it.
        /// </summary>
        [Fact]
        public void ClientKey_CorrelatesNewColumnAndPreservesWidgets()
        {
            Seed(nameof(ClientKey_CorrelatesNewColumnAndPreservesWidgets));

            var dashboard = SampleWithBoard("Board");
            CoreHub.DashboardManager.Add(dashboard);

            var first = dashboard.Columns[0];
            var second = dashboard.Columns[1];
            const string clientKey = "col_session_new";

            // add a new column via a column-only update (empty id, transient client key)
            CoreHub.DashboardManager.SetColumns(dashboard.Id,
            [
                new DashboardColumn(first.Id) { Name = first.Name, Size = "1fr" },
                new DashboardColumn(second.Id) { Name = second.Name, Size = "1fr" },
                new DashboardColumn(Guid.Empty) { Key = clientKey, Name = "New column", Size = "1fr" }
            ]);

            Assert.Equal(3, CoreHub.DashboardManager.GetDashboard(dashboard.Id).Columns.Count);

            // add a widget to the still-transient column via a board update (client still uses the key)
            CoreHub.DashboardManager.SetBoard(dashboard.Id,
            [
                new DashboardColumn(first.Id) { Name = first.Name, Size = "1fr", Widgets = [WidgetOf("widget_info", "Left")] },
                new DashboardColumn(second.Id) { Name = second.Name, Size = "1fr", Widgets = [WidgetOf("widget_info", "Right")] },
                new DashboardColumn(Guid.Empty) { Key = clientKey, Name = "New column", Size = "1fr", Widgets = [WidgetOf("widget_kleenestar_note", "Note")] }
            ]);

            var afterBoard = CoreHub.DashboardManager.GetDashboard(dashboard.Id);
            Assert.Equal(3, afterBoard.Columns.Count);
            Assert.Single(afterBoard.Columns.Single(c => c.Key == clientKey).Widgets);

            // a later column-only update (reorder) must keep the keyed column's widget
            CoreHub.DashboardManager.SetColumns(dashboard.Id,
            [
                new DashboardColumn(Guid.Empty) { Key = clientKey, Name = "New column", Size = "1fr" },
                new DashboardColumn(first.Id) { Name = first.Name, Size = "1fr" },
                new DashboardColumn(second.Id) { Name = second.Name, Size = "1fr" }
            ]);

            var afterReorder = CoreHub.DashboardManager.GetDashboard(dashboard.Id);
            Assert.Equal(3, afterReorder.Columns.Count);
            var keyed = afterReorder.Columns.Single(c => c.Key == clientKey);
            Assert.Single(keyed.Widgets);
            Assert.Equal(0, keyed.Position);
        }

        /// <summary>
        /// Creates a detached widget of the given type and name for use in board test payloads.
        /// </summary>
        /// <param name="type">The widget registry type id.</param>
        /// <param name="name">The widget name.</param>
        /// <returns>The widget.</returns>
        private static Widget WidgetOf(string type, string name) => new(Guid.NewGuid())
        {
            Type = type,
            Name = name
        };

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

        /// <summary>
        /// Creates a sample <see cref="Dashboard"/> with two columns, each carrying one widget, so
        /// the column and board persistence paths have a graph to operate on.
        /// </summary>
        /// <param name="name">The dashboard name.</param>
        /// <returns>The sample dashboard with a seeded board.</returns>
        private static Dashboard SampleWithBoard(string name)
        {
            var dashboard = Sample(name);

            var left = new DashboardColumn(Guid.NewGuid())
            {
                Name = "Left",
                Size = "1fr",
                Position = 0,
                DashboardId = dashboard.Id
            };
            left.Widgets.Add(new Widget(Guid.NewGuid())
            {
                Type = "widget_info",
                Name = "Left Widget",
                Position = 0,
                ColumnId = left.Id
            });

            var right = new DashboardColumn(Guid.NewGuid())
            {
                Name = "Right",
                Size = "1fr",
                Position = 1,
                DashboardId = dashboard.Id
            };
            right.Widgets.Add(new Widget(Guid.NewGuid())
            {
                Type = "widget_info",
                Name = "Right Widget",
                Position = 0,
                ColumnId = right.Id
            });

            dashboard.Columns = [left, right];

            return dashboard;
        }
    }
}
