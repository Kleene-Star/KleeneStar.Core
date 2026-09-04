using KleeneStar.Core.WebWorkspaceTemplate;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebLog;
using WebExpress.WebCore.WebPlugin;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Manages the workspace templates the installed plugins define.
    /// </summary>
    /// <remarks>
    /// The registration follows the framework's <c>FragmentManager</c>: the plugin manager is the
    /// source of truth about what is installed, so this manager subscribes to it rather than
    /// scanning the process, and it keeps its registrations keyed by plugin so a plugin that goes
    /// away takes exactly its own templates with it. Discovery is by interface rather than by
    /// attribute, because a template has nothing to declare beyond being one.
    /// </remarks>
    public sealed class WorkspaceTemplateManager : IWorkspaceTemplateManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;

        /// <summary>
        /// The registrations, keyed by the plugin that defines them. A dictionary per plugin
        /// rather than one flat list, because removal is per plugin and has to be exact.
        /// </summary>
        private readonly ConcurrentDictionary<IPluginContext, List<IWorkspaceTemplateContext>> _dictionary = new();

        /// <summary>
        /// Raised after a template has been registered.
        /// </summary>
        public event EventHandler<IWorkspaceTemplateContext> AddWorkspaceTemplate;

        /// <summary>
        /// Raised after a template has been dropped.
        /// </summary>
        public event EventHandler<IWorkspaceTemplateContext> RemoveWorkspaceTemplate;

        /// <summary>
        /// Gets every registered template, in catalogue order.
        /// </summary>
        public IEnumerable<IWorkspaceTemplateContext> WorkspaceTemplates => Order(_dictionary.Values.SelectMany(x => x));

        /// <summary>
        /// Initializes a new instance of the manager. Invoked by WebExpress via reflection.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The HTTP server context.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via reflection.")]
        private WorkspaceTemplateManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;

            if (_componentHub?.PluginManager is not null)
            {
                _componentHub.PluginManager.AddPlugin += OnAddPlugin;
                _componentHub.PluginManager.RemovePlugin += OnRemovePlugin;

                // the manager is built while the core's own components are registered, which is
                // before every other plugin has arrived - so the plugins already known are swept
                // here and the rest reach it through the event above. Neither pass alone is
                // enough: the sweep misses what comes later, the event misses what came first.
                foreach (var pluginContext in _componentHub.PluginManager.Plugins)
                {
                    Register(pluginContext);
                }
            }
        }

        /// <summary>
        /// Discovers the templates a plugin defines and registers them.
        /// </summary>
        /// <param name="pluginContext">The plugin to scan.</param>
        private void Register(IPluginContext pluginContext)
        {
            if (pluginContext?.Assembly is null || _dictionary.ContainsKey(pluginContext))
            {
                return;
            }

            var registered = new List<IWorkspaceTemplateContext>();

            foreach (var templateType in GetTemplateTypes(pluginContext))
            {
                var template = Instantiate(templateType);

                if (template is null || string.IsNullOrWhiteSpace(template.Key))
                {
                    continue;
                }

                registered.Add(new WorkspaceTemplateContext
                {
                    PluginContext = pluginContext,
                    TemplateType = templateType,
                    Template = template
                });
            }

            if (registered.Count == 0)
            {
                return;
            }

            _dictionary[pluginContext] = registered;

            Log(pluginContext, registered);

            foreach (var context in registered)
            {
                AddWorkspaceTemplate?.Invoke(this, context);
            }
        }

        /// <summary>
        /// Returns the template types of a plugin.
        /// </summary>
        /// <remarks>
        /// A plugin whose types cannot all be loaded is not a reason to lose the ones that can:
        /// an assembly referencing something absent throws on the whole set, and the templates
        /// that resolved are still valid.
        /// </remarks>
        /// <param name="pluginContext">The plugin to scan.</param>
        /// <returns>The candidate types.</returns>
        private static IEnumerable<Type> GetTemplateTypes(IPluginContext pluginContext)
        {
            Type[] types;

            try
            {
                types = pluginContext.Assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                types = [.. ex.Types.Where(x => x is not null)];
            }

            return types
                .Where(x => x.IsClass && !x.IsAbstract && x.IsPublic)
                .Where(x => typeof(IWorkspaceTemplate).IsAssignableFrom(x));
        }

        /// <summary>
        /// Instantiates a template type.
        /// </summary>
        /// <param name="templateType">The type to instantiate.</param>
        /// <returns>The template, or <see langword="null"/> when it could not be created.</returns>
        private IWorkspaceTemplate Instantiate(Type templateType)
        {
            try
            {
                return Activator.CreateInstance(templateType) as IWorkspaceTemplate;
            }
            catch (Exception ex)
            {
                // a template that throws in its constructor is a defect of the plugin that ships
                // it, and the rest of the catalogue must still be offered
                _httpServerContext?.Log?.Warning(templateType.FullName + ": " + ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Drops the registrations of a plugin.
        /// </summary>
        /// <param name="pluginContext">The plugin that was removed.</param>
        private void Remove(IPluginContext pluginContext)
        {
            if (pluginContext is null || !_dictionary.TryRemove(pluginContext, out var removed))
            {
                return;
            }

            foreach (var context in removed)
            {
                RemoveWorkspaceTemplate?.Invoke(this, context);
            }
        }

        /// <summary>
        /// Handles the arrival of a plugin.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The plugin context.</param>
        private void OnAddPlugin(object sender, IPluginContext e)
        {
            Register(e);
        }

        /// <summary>
        /// Handles the removal of a plugin.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The plugin context.</param>
        private void OnRemovePlugin(object sender, IPluginContext e)
        {
            Remove(e);
        }

        /// <summary>
        /// Returns the templates the supplied plugin defines.
        /// </summary>
        /// <param name="pluginContext">The plugin.</param>
        /// <returns>Its templates, in catalogue order.</returns>
        public IEnumerable<IWorkspaceTemplateContext> GetWorkspaceTemplates(IPluginContext pluginContext)
        {
            return pluginContext is not null && _dictionary.TryGetValue(pluginContext, out var templates)
                ? Order(templates)
                : [];
        }

        /// <summary>
        /// Returns the template with the supplied key.
        /// </summary>
        /// <param name="key">The stable key of the template.</param>
        /// <returns>The registration, or <see langword="null"/>.</returns>
        public IWorkspaceTemplateContext GetWorkspaceTemplate(string key)
        {
            return string.IsNullOrWhiteSpace(key)
                ? null
                : WorkspaceTemplates.FirstOrDefault(x => string.Equals(x.Template.Key, key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Creates the classes the supplied template describes in the supplied workspace.
        /// </summary>
        /// <remarks>
        /// A name the workspace already carries is skipped rather than duplicated, so applying a
        /// template twice - a retried create, a template applied to a workspace that was set up
        /// by hand - adds what is missing instead of a second set of everything.
        /// </remarks>
        /// <param name="key">The stable key of the template to apply.</param>
        /// <param name="workspaceId">The workspace the classes are created in.</param>
        /// <returns>The classes created.</returns>
        public IReadOnlyList<Class> Apply(string key, Guid workspaceId)
        {
            var template = GetWorkspaceTemplate(key)?.Template;

            if (template is null || workspaceId == Guid.Empty || CoreHub.WorkspaceManager.GetWorkspace(workspaceId) is null)
            {
                return [];
            }

            var existing = CoreHub.ClassManager
                .GetClasses(new WebExpress.WebIndex.Queries.Query<Class>().WhereEquals(x => x.WorkspaceId, workspaceId))
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var created = new List<Class>();

            foreach (var descriptor in template.Classes ?? [])
            {
                if (string.IsNullOrWhiteSpace(descriptor.Name) || !existing.Add(descriptor.Name))
                {
                    continue;
                }

                var @class = new Class
                {
                    Name = descriptor.Name,
                    Description = descriptor.Description,
                    Icon = string.IsNullOrWhiteSpace(descriptor.Icon) ? null : ImageIcon.FromString(descriptor.Icon),
                    Kind = descriptor.Kind,
                    PortalVisible = descriptor.PortalVisible,
                    Sealed = descriptor.Sealed,
                    AccessModifier = descriptor.AccessModifier,
                    WorkspaceId = workspaceId,
                    State = ClassState.Active,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                CoreHub.ClassManager.Add(@class);
                created.Add(@class);
            }

            return created;
        }

        /// <summary>
        /// Puts a set of registrations into the order they are offered in.
        /// </summary>
        /// <param name="templates">The registrations.</param>
        /// <returns>The ordered registrations.</returns>
        private static IEnumerable<IWorkspaceTemplateContext> Order(IEnumerable<IWorkspaceTemplateContext> templates)
        {
            return [.. templates
                .OrderBy(x => x.Template.Order)
                .ThenBy(x => x.Template.Key, StringComparer.OrdinalIgnoreCase)];
        }

        /// <summary>
        /// Announces what a plugin contributed, the way the framework's managers announce what
        /// they found.
        /// </summary>
        /// <remarks>
        /// Per plugin rather than once over the whole catalogue, because the catalogue is never
        /// complete at one moment: this manager is built while the core registers its components,
        /// and every plugin after that arrives through an event.
        /// </remarks>
        /// <param name="pluginContext">The plugin that contributed.</param>
        /// <param name="templates">Its templates.</param>
        private void Log(IPluginContext pluginContext, IEnumerable<IWorkspaceTemplateContext> templates)
        {
            if (_httpServerContext?.Log is null)
            {
                return;
            }

            using var frame = new LogFrameSimple(_httpServerContext.Log);

            var lines = new List<string> { "Workspace templates of '" + pluginContext.PluginId + "':" };

            lines.AddRange(Order(templates).Select(x => string.Empty.PadRight(2) + x.Template.Key));

            _httpServerContext.Log.Info(string.Join(Environment.NewLine, lines));
        }

        /// <summary>
        /// Releases unmanaged resources reserved during use.
        /// </summary>
        public void Dispose()
        {
            if (_componentHub?.PluginManager is not null)
            {
                _componentHub.PluginManager.AddPlugin -= OnAddPlugin;
                _componentHub.PluginManager.RemovePlugin -= OnRemovePlugin;
            }

            GC.SuppressFinalize(this);
        }
    }
}
