using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;
using WebExpress.WebUI.WebSection;

// the fragment exposes a Quickfilter property of its own, so the endpoint of the same name is
// reached through an alias rather than through a qualified name inside every reference
using IssueQuickfilterApi = KleeneStar.Core.WWW.Api._1_.Issues._workspacekey_.Quickfilter;

namespace KleeneStar.Core.WebFragment.Object.Issues
{
    /// <summary>
    /// Quickfilter header of the issue overview: offers the personal scope chips
    /// (starred, assigned to me, created by me, archived) served by the issue
    /// quickfilter endpoint.
    /// </summary>
    [Section<SectionViewHeaderSecondary>]
    [Scope<IssueTabViewFragment>]
    [Cache]
    public sealed class IssueTabViewQuickfilterFragment : FragmentControlViewHeader
    {
        /// <summary>
        /// Represents the unique identifier for the content.
        /// </summary>
        public static readonly string ContentId = "id_9D4F1C22A31E4E3E8B7B54E11F0C6B42";

        /// <summary>
        /// Gets the quick filter control for the REST-based issue queries.
        /// </summary>
        public ControlDataQuickfilter Quickfilter { get; } = new ControlDataQuickfilter(ContentId)
        {
            ServiceFactory = _ => DataServiceDescriptor.QueryData(CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Issues._workspacekey_.Quickfilter>().ToString())
        };

        /// <summary>
        /// Gets the chip that opens the dialog in which a new quickfilter is defined.
        /// </summary>
        /// <remarks>
        /// The chip carries no filter and never shows active; the client keeps it at the trailing
        /// edge of the bar. The workspace travels along on the dialog address, so a filter defined
        /// here belongs to this workspace's issue list rather than to every workspace's.
        /// </remarks>
        public ControlQuickfilterItemAdd AddFilter { get; } = new()
        {
            Tooltip = _ => "kleenestar.core:quickfilter.add.label",
            PrimaryAction = renderContext => new ActionModal
            (
                "modal-form",
                CoreHub.GetUri<global::KleeneStar.Core.WWW.Quickfilters.Add>()
                    .BindParameters(renderContext.Request)
                    .Concat($"?view={IssueQuickfilterApi.ViewKey}&context={renderContext.Request?.GetParameter<WorkspaceKeyParameter>()?.Value}"),
                TypeModalSize.Large
            )
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public IssueTabViewQuickfilterFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Quickfilter.Add(AddFilter);

            Add(Quickfilter);
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
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree);
        }
    }
}
