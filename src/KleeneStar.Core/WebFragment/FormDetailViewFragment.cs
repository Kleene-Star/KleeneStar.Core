using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a fragment control that displays the three fixed forms (create, edit, view) per
    /// class within a ControlView. This fragment is only rendered for standard forms. Additional
    /// forms do not have these predefined views and will display an empty or custom layout instead.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Form._formid_.Index>]
    [Cache]
    public sealed class FormDetailViewFragment : FragmentControlRestFormEditor
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public FormDetailViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <remarks>
        /// The three predefined views (create, edit, view) are only rendered for standard forms.
        /// Additional forms do not display these tabs as they serve as flexible UI masks with
        /// their own layouts.
        /// </remarks>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var formIdParam = renderContext.Request.GetParameter<FormIdParameter>();

            var restUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Forms.FormEditor>()?
                .Add(new UriQuery("id", formIdParam?.Value.ToString()))
                .BindParameters(formIdParam)
                .BindParameters(renderContext.Request);

            return base.Render(renderContext, visualTree);
        }
    }
}
