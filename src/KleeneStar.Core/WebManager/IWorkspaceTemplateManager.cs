using KleeneStar.Core.WebWorkspaceTemplate;
using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebPlugin;

namespace KleeneStar.Core.WebManager
{
    /// <summary>
    /// Manages the workspace templates the installed plugins define.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is modelled on the framework's <c>FragmentManager</c> and works the same way: it
    /// listens to the plugin manager, scans the assembly of every plugin that arrives for types
    /// implementing <see cref="IWorkspaceTemplate"/>, keeps one registration per type, and drops
    /// the registrations of a plugin again when that plugin is removed. Nothing is written back;
    /// a template is code, and the catalogue is only ever as long as the set of installed
    /// plugins says.
    /// </para>
    /// <para>
    /// Unlike a fragment, a template is <b>not</b> bound to an application. A fragment is a piece
    /// of one particular page and only means something inside the application that page belongs
    /// to; a workspace template is a description of a workspace, and a workspace is the
    /// installation's, not an application's.
    /// </para>
    /// </remarks>
    public interface IWorkspaceTemplateManager : IComponentManager
    {
        /// <summary>
        /// Raised after a template has been registered.
        /// </summary>
        event EventHandler<IWorkspaceTemplateContext> AddWorkspaceTemplate;

        /// <summary>
        /// Raised after a template has been dropped, because the plugin that defined it was
        /// removed.
        /// </summary>
        event EventHandler<IWorkspaceTemplateContext> RemoveWorkspaceTemplate;

        /// <summary>
        /// Gets every registered template, ordered by <see cref="IWorkspaceTemplate.Order"/> and
        /// then by key.
        /// </summary>
        IEnumerable<IWorkspaceTemplateContext> WorkspaceTemplates { get; }

        /// <summary>
        /// Returns the templates the supplied plugin defines.
        /// </summary>
        /// <param name="pluginContext">The plugin.</param>
        /// <returns>Its templates, in catalogue order. The collection may be empty.</returns>
        IEnumerable<IWorkspaceTemplateContext> GetWorkspaceTemplates(IPluginContext pluginContext);

        /// <summary>
        /// Returns the template with the supplied key.
        /// </summary>
        /// <param name="key">The stable key of the template. May be null.</param>
        /// <returns>The registration, or <see langword="null"/> when no plugin defines it - which
        /// is the ordinary answer for a workspace whose template has since been uninstalled.</returns>
        IWorkspaceTemplateContext GetWorkspaceTemplate(string key);

        /// <summary>
        /// Creates the classes the supplied template describes in the supplied workspace.
        /// </summary>
        /// <remarks>
        /// This is the one thing the manager does beyond answering questions, and it is here
        /// rather than at the call site because applying a template is the same act wherever it
        /// is triggered from - the creation wizard today, an import or a scripted setup
        /// tomorrow - and it is the manager that knows what a registration means.
        /// </remarks>
        /// <param name="key">The stable key of the template to apply.</param>
        /// <param name="workspaceId">The workspace the classes are created in.</param>
        /// <returns>The classes created. Empty when the template or the workspace is unknown, or
        /// when the workspace already carries classes of the same names.</returns>
        IReadOnlyList<Class> Apply(string key, Guid workspaceId);
    }
}
