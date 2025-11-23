using System;
using System.Collections.Generic;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WebWorkspace
{
    /// <summary>
    /// Represents a workspace, which serves as a container for related resources and metadata.
    /// </summary>
    public class Workspace : IWorkspace
    {
        [RestTableColumnHidden()]
        public Guid Id { get; set; }

        /// <summary>
        /// Returns or sets the key of the workspace.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Returns the name of the workspace.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the collection of category names associated with the item.
        /// </summary>
        public IEnumerable<string> Categories { get; set; }

        /// <summary>
        /// Returns the current state of the workspace.
        /// </summary>
        public TypeWorkspaceState State { get; set; }

        /// <summary>
        /// Returns the description of the workspace.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns the URI associated with the current workspace view.
        /// </summary>
        [RestTableColumnHidden()]
        public IUri Uri => KleeneStar.GetUri<WWW.Index>().Concat(Key);

        /// <summary>
        /// Returns the icon associated with this workspace.
        /// </summary>
        [RestTableColumnHidden()]
        public IIcon Icon { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Workspace()
        {
            Id = Guid.NewGuid();
        }
    }
}
