using System.Collections.Generic;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebCore.WebDomain;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex;

namespace KleeneStar.Core.Model.Workspace
{
    /// <summary>
    /// Represents a workspace that provides a unique key, name, and description.
    /// </summary>
    /// <remarks>
    public interface IWorkspace : IIndexItem, IDomain
    {
        /// <summary>
        /// Returns the key of the workspace.
        /// </summary>
        [RestTableColumnName("Key")]
        string Key { get; }

        /// <summary>
        /// Returns the name of the workspace.
        /// </summary>
        [RestTableColumnName("Name")]
        [RestDropdownText]
        string Name { get; }

        /// <summary>
        /// Returns the collection of category names associated with the item.
        /// </summary>
        [RestTableColumnName("Category")]
        [RestApiTableColumnTemplateTag(true)]
        [RestTableJoin(';')]
        IEnumerable<string> Categories { get; }

        /// <summary>
        /// Returns the description of the workspace.
        /// </summary>
        [RestTableColumnName("Description")]
        string Description { get; }

        /// <summary>
        /// Returns the current state of the workspace.
        /// </summary>
        [RestTableColumnName("State")]
        TypeWorkspaceState State { get; }

        /// <summary>
        /// Returns the URI associated with the current workspace view.
        /// </summary>
        [RestTableColumnHidden]
        [RestTableRowUri]
        [RestDropdownUri]
        IUri Uri { get; }

        /// <summary>
        /// Returns the icon associated with this workspace.
        /// </summary>
        [RestTableRowIcon]
        [RestDropdownIcon]
        IIcon Icon { get; }
    }
}