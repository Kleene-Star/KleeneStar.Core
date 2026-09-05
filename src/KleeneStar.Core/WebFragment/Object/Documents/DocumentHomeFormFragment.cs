using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object.Documents
{
    /// <summary>
    /// The form behind the home-page picker of a workspace's document overview: one selection,
    /// naming the document the overview opens on.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It edits the <b>workspace</b>, not the document - the home page is a property of the
    /// workspace, and everybody opening the overview lands on the same one - so it submits to the
    /// workspace CRUD endpoint with the workspace resolved from the route, exactly as
    /// <see cref="Workspace.WorkspaceEditFormFragment"/> does. There is no endpoint of its own:
    /// a single-property form of an entity that already has a CRUD surface does not need one.
    /// </para>
    /// <para>
    /// The selection's first entry stands for "no choice" and carries the empty guid, which the
    /// form binder reads as "clear this property" - so the same control both chooses a page and
    /// gives it back up, and the overview returns to the first root of the page tree.
    /// </para>
    /// </remarks>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Documents._workspacekey_.Home>]
    [Cache]
    public sealed class DocumentHomeFormFragment : FragmentControlDataFormEdit
    {
        /// <summary>
        /// Gets the input selection control naming the document the overview opens on.
        /// </summary>
        public ControlDataFormItemInputSelection Home { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Workspace.HomeId),
            Label = _ => "kleenestar.core:workspace.home.label",

            Placeholder = _ => "kleenestar.core:workspace.home.placeholder",
            Help = _ => "kleenestar.core:workspace.home.help",
            ServiceFactory = renderContext => DataServiceDescriptor.QueryData
            (
                CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Documents._workspacekey_.Selection>()?
                    .BindParameters(renderContext.Request)
                    .ToString()
            )
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public DocumentHomeFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Home);

            this.DataService<global::KleeneStar.Core.WWW.Api._1_.Workspaces.Index>();

            ItemId = renderContext =>
            {
                var key = renderContext.Request.GetParameter<WorkspaceKeyParameter>();

                return CoreHub.WorkspaceManager.GetWorkspaceByKey(key?.Value)?.Id.ToString();
            };
        }

        /// <summary>
        /// Renders the control as an HTML node.
        /// </summary>
        /// <param name="renderContext">
        /// The context in which the control is rendered.
        /// </param>
        /// <param name="visualTree">
        /// The visual tree representing the control's structure.
        /// </param>
        /// <returns>
        /// An HTML node representing the rendered control.
        /// </returns>
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
