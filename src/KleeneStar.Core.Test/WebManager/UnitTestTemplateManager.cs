using KleeneStar.Core.Test;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.Test.WebManager
{
    /// <summary>
    /// Provides unit tests for <see cref="KleeneStar.Core.WebManager.TemplateManager"/>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestTemplateManager
    {
        private static readonly Guid WorkspaceId = Guid.Parse("66AABBCC-DDEE-FF00-1122-334455667788");
        private static readonly Guid ClassId = Guid.Parse("77BBCCDD-EEFF-0011-2233-445566778899");

        /// <summary>
        /// Seeds the in-memory database with the workspace and class that templates attach to.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        private static void Seed(string connectionString)
        {
            CoreHubFixture.Initialize(connectionString);

            using var db = CoreHubFixture.CreateDbContext(connectionString);

            if (!db.Workspaces.Any(x => x.Id == WorkspaceId))
            {
                db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-tmpl", Name = "main" });
            }
            if (!db.Classes.Any(x => x.Id == ClassId))
            {
                db.Classes.Add(new Class { Id = ClassId, Name = "Incident", WorkspaceId = WorkspaceId });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies that <c>AddTemplate</c> persists the template and that <c>GetTemplate</c>
        /// retrieves it by its business id.
        /// </summary>
        [Fact]
        public void AddTemplate_Then_GetTemplate_RoundTrip()
        {
            Seed(nameof(AddTemplate_Then_GetTemplate_RoundTrip));

            var template = Sample("Quickstart");
            CoreHub.TemplateManager.AddTemplate(template);

            var loaded = CoreHub.TemplateManager.GetTemplate(template.Id);

            Assert.NotNull(loaded);
            Assert.Equal("Quickstart", loaded.Name);
        }

        /// <summary>
        /// Verifies that <c>UpdateTemplate</c> writes scalar property changes back to the database.
        /// </summary>
        [Fact]
        public void UpdateTemplate_ChangesScalars()
        {
            Seed(nameof(UpdateTemplate_ChangesScalars));

            var template = Sample("Initial");
            CoreHub.TemplateManager.AddTemplate(template);

            template.Name = "Renamed";
            CoreHub.TemplateManager.UpdateTemplate(template);

            var loaded = CoreHub.TemplateManager.GetTemplate(template.Id);
            Assert.NotNull(loaded);
            Assert.Equal("Renamed", loaded.Name);
        }

        /// <summary>
        /// Verifies that <c>RemoveTemplate</c> deletes the template and raises the
        /// <see cref="KleeneStar.Core.WebManager.ITemplateManager.TemplateRemoved"/> event.
        /// </summary>
        [Fact]
        public void RemoveTemplate_DeletesAndRaisesEvent()
        {
            Seed(nameof(RemoveTemplate_DeletesAndRaisesEvent));

            var template = Sample("DeleteMe");
            CoreHub.TemplateManager.AddTemplate(template);

            Template? raised = null;
            CoreHub.TemplateManager.TemplateRemoved += (_, t) => raised = t;

            CoreHub.TemplateManager.RemoveTemplate(template);

            Assert.Null(CoreHub.TemplateManager.GetTemplate(template.Id));
            Assert.NotNull(raised);
            Assert.Equal(template.Id, raised.Id);
        }

        /// <summary>
        /// Verifies that <c>GetTemplates(IQuery)</c> returns templates from the database.
        /// </summary>
        [Fact]
        public void GetTemplates_ReturnsAllStored()
        {
            Seed(nameof(GetTemplates_ReturnsAllStored));

            CoreHub.TemplateManager.AddTemplate(Sample("Alpha"));
            CoreHub.TemplateManager.AddTemplate(Sample("Beta"));

            var result = CoreHub.TemplateManager.GetTemplates(new Query<Template>()).ToList();

            Assert.True(result.Count >= 2);
            Assert.Contains(result, t => t.Name == "Alpha");
            Assert.Contains(result, t => t.Name == "Beta");
        }

        /// <summary>
        /// Creates a sample <see cref="Template"/> attached to the seeded class.
        /// </summary>
        /// <param name="name">The template name.</param>
        /// <returns>The sample template.</returns>
        private static Template Sample(string name) => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            ClassId = ClassId,
            State = TemplateState.Active
        };
    }
}
