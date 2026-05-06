using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a control fragment that provides a button link for adding a new form within the workspace.
    /// </summary>
    [Section<SectionHeadlinePrimary>]
    [Scope<global::KleeneStar.Core.WWW.Forms._classid_.Index>]
    [Cache]
    public sealed class FormAddButtonFragment : FragmentControlButtonLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public FormAddButtonFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = _ => "kleenestar.core:form.add.label";
            Icon = _ => new IconPlus(TypeIconTheme.Light);
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = _ => new PropertyColorButton(TypeColorButton.Primary);
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        /// <param name="renderContext">The context in which the fragment is rendered.</param>
        /// <param name="visualTree">The visual tree used for rendering the fragment.</param>
        /// <returns>An HTML node representing the rendered fragments. Can be null if no nodes are present.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var primaryAction = new ActionModal
            (
                "modal-form",
                CoreHub.GetUri<global::KleeneStar.Core.WWW.Forms._classid_.Add>()
                    .BindParameters(renderContext.Request),
                TypeModalSize.ExtraLarge
                );

            return base.Render(renderContext, visualTree);
        }
    }
}
