using KleeneStar.Core.WebWorkspaceTemplate;
using KleeneStar.Model.Entities;
using System.Reflection;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebEndpoint;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebPlugin;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebIcon;
using ClassEntity = KleeneStar.Model.Entities.Class;

namespace KleeneStar.Core.Test.WebManager
{
    /// <summary>
    /// Provides unit tests for <see cref="KleeneStar.Core.WebManager.WorkspaceTemplateManager"/>.
    /// </summary>
    /// <remarks>
    /// Discovery is driven by the framework's plugin manager, which the fixture does not stand
    /// up - the tests therefore exercise what the manager does with a template rather than how it
    /// finds one, which is the half that carries the rules: applying a template creates its
    /// classes, applying it twice does not create them again, and an unknown key creates nothing.
    /// The discovery half is verified against the running host.
    /// </remarks>
    [Collection("NonParallelTests")]
    public class UnitTestWorkspaceTemplateManager
    {
        private static readonly Guid WorkspaceId = Guid.Parse("B7C8D9E0-1111-4111-8111-111111111111");
        private static readonly Guid OtherWorkspaceId = Guid.Parse("B7C8D9E0-2222-4222-8222-222222222222");

        /// <summary>
        /// A template with two classes, one of them a document, standing in for the ones a plugin
        /// would ship.
        /// </summary>
        private sealed class ProbeTemplate : IWorkspaceTemplate
        {
            public string Key => "test.probe";

            public string Name => "Probe";

            public string Description => "A probe.";

            public IIcon Icon => ImageIcon.FromString("/kleenestar/assets/icons/sd.svg");

            public string SuggestedKey => "PRB";

            public IEnumerable<string> Categories => ["Support"];

            public int Order => 1;

            public IEnumerable<WorkspaceTemplateClass> Classes =>
            [
                new WorkspaceTemplateClass
                {
                    Name = "Ticket",
                    Description = "Requests as they arrive.",
                    Icon = "/kleenestar/assets/icons/ticket.svg",
                    PortalVisible = true
                },
                new WorkspaceTemplateClass
                {
                    Name = "Knowledge",
                    Description = "The written answer.",
                    Icon = "/kleenestar/assets/icons/knowledge.svg",
                    Kind = ObjectKind.Document
                }
            ];
        }

        /// <summary>
        /// Seeds two workspaces and registers the probe template with the manager.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        private static void Seed(string connectionString)
        {
            CoreHubFixture.Initialize(connectionString);

            using var db = CoreHubFixture.CreateDbContext(connectionString);

            db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-tpl", Name = "templated" });
            db.Workspaces.Add(new Workspace { Id = OtherWorkspaceId, Key = "ws-oth", Name = "other" });
            db.SaveChanges();

            Register(new ProbeTemplate());
        }

        /// <summary>
        /// Puts a template into the manager's registry.
        /// </summary>
        /// <remarks>
        /// The registry is filled from the plugin manager, which the fixture has no instance of,
        /// so the registration is written straight into the private dictionary. That is the seam
        /// the tests need and the one place they may reach through it: everything else they
        /// assert goes through the public surface.
        /// </remarks>
        /// <param name="template">The template to register.</param>
        private static void Register(IWorkspaceTemplate template)
        {
            var manager = CoreHub.WorkspaceTemplateManager;

            var field = manager.GetType().GetField("_dictionary", BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new InvalidOperationException("The registry field was renamed.");

            var dictionary = field.GetValue(manager)
                ?? throw new InvalidOperationException("The registry is null.");

            var contextType = typeof(CoreHub).Assembly.GetType("KleeneStar.Core.WebWorkspaceTemplate.WorkspaceTemplateContext")
                ?? throw new InvalidOperationException("The context type was renamed.");

            var context = Activator.CreateInstance(contextType);
            contextType.GetProperty("Template")!.SetValue(context, template);
            contextType.GetProperty("TemplateType")!.SetValue(context, template.GetType());

            var listType = typeof(List<>).MakeGenericType(typeof(IWorkspaceTemplateContext));
            var list = (System.Collections.IList)Activator.CreateInstance(listType)!;
            list.Add(context);

            // the key of the registry is the plugin, and there is none in a unit test - a null
            // key is refused by the concurrent dictionary, so a stand-in plugin context is used
            var pluginContext = new StubPluginContext();

            dictionary.GetType().GetMethod("TryAdd")!.Invoke(dictionary, [pluginContext, list]);
        }

        /// <summary>
        /// The plugin a registered template is filed under in a test.
        /// </summary>
        private sealed class StubPluginContext : IPluginContext
        {
            public IComponentId PluginId => null;
            public string PluginName => "test";
            public string Description => null;
            public string Manufacturer => null;
            public string Copyright => null;
            public string Version => null;
            public string License => null;
            public IRoute Icon => null;
            public Assembly Assembly => typeof(UnitTestWorkspaceTemplateManager).Assembly;
        }

        /// <summary>
        /// A registered template is answered by its key and appears in the catalogue.
        /// </summary>
        [Fact]
        public void RegisteredTemplateIsFound()
        {
            Seed(nameof(RegisteredTemplateIsFound));

            Assert.Contains(CoreHub.WorkspaceTemplateManager.WorkspaceTemplates, x => x.Template.Key == "test.probe");

            var found = CoreHub.WorkspaceTemplateManager.GetWorkspaceTemplate("test.probe");

            Assert.NotNull(found);
            Assert.Equal("PRB", found.Template.SuggestedKey);
        }

        /// <summary>
        /// The lookup is case-insensitive and answers nothing for an unknown key - which is the
        /// ordinary answer for a workspace whose template has since been uninstalled, not an
        /// error.
        /// </summary>
        [Fact]
        public void UnknownTemplateIsNull()
        {
            Seed(nameof(UnknownTemplateIsNull));

            Assert.NotNull(CoreHub.WorkspaceTemplateManager.GetWorkspaceTemplate("TEST.PROBE"));
            Assert.Null(CoreHub.WorkspaceTemplateManager.GetWorkspaceTemplate("test.gone"));
            Assert.Null(CoreHub.WorkspaceTemplateManager.GetWorkspaceTemplate(null));
        }

        /// <summary>
        /// Applying a template creates its classes in the workspace, with the kind and the portal
        /// visibility the template declared.
        /// </summary>
        [Fact]
        public void ApplyCreatesTheClasses()
        {
            Seed(nameof(ApplyCreatesTheClasses));

            var created = CoreHub.WorkspaceTemplateManager.Apply("test.probe", WorkspaceId);

            Assert.Equal(2, created.Count);

            var classes = CoreHub.ClassManager
                .GetClasses(new Query<ClassEntity>().WhereEquals(x => x.WorkspaceId, WorkspaceId))
                .ToList();

            var ticket = classes.Single(x => x.Name == "Ticket");
            var knowledge = classes.Single(x => x.Name == "Knowledge");

            Assert.True(ticket.PortalVisible);
            Assert.Equal(ObjectKind.Issue, ticket.Kind);
            Assert.Equal(ObjectKind.Document, knowledge.Kind);
            Assert.Equal(ClassState.Active, knowledge.State);
        }

        /// <summary>
        /// Applying the same template twice adds what is missing rather than a second set of
        /// everything - a retried create, or a template applied to a workspace somebody had
        /// already set up by hand, must not double its classes.
        /// </summary>
        [Fact]
        public void ApplyTwiceIsIdempotent()
        {
            Seed(nameof(ApplyTwiceIsIdempotent));

            CoreHub.WorkspaceTemplateManager.Apply("test.probe", WorkspaceId);
            var second = CoreHub.WorkspaceTemplateManager.Apply("test.probe", WorkspaceId);

            Assert.Empty(second);
            Assert.Equal(2, CoreHub.ClassManager
                .GetClasses(new Query<ClassEntity>().WhereEquals(x => x.WorkspaceId, WorkspaceId))
                .Count());
        }

        /// <summary>
        /// The classes land in the workspace they were applied to and nowhere else.
        /// </summary>
        [Fact]
        public void ApplyTouchesOnlyItsWorkspace()
        {
            Seed(nameof(ApplyTouchesOnlyItsWorkspace));

            CoreHub.WorkspaceTemplateManager.Apply("test.probe", WorkspaceId);

            Assert.Empty(CoreHub.ClassManager
                .GetClasses(new Query<ClassEntity>().WhereEquals(x => x.WorkspaceId, OtherWorkspaceId)));
        }

        /// <summary>
        /// An unknown template or an unknown workspace creates nothing and throws nothing: the
        /// create endpoint applies whatever the payload named, and a payload naming an
        /// uninstalled template must still produce a workspace.
        /// </summary>
        [Fact]
        public void ApplyUnknownCreatesNothing()
        {
            Seed(nameof(ApplyUnknownCreatesNothing));

            Assert.Empty(CoreHub.WorkspaceTemplateManager.Apply("test.gone", WorkspaceId));
            Assert.Empty(CoreHub.WorkspaceTemplateManager.Apply("test.probe", Guid.NewGuid()));
            Assert.Empty(CoreHub.WorkspaceTemplateManager.Apply("test.probe", Guid.Empty));
            Assert.Empty(CoreHub.WorkspaceTemplateManager.Apply(null, WorkspaceId));
        }
    }
}
