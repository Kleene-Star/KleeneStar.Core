using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a control fragment that provides a button link for edit a class within the workspace.
    /// </summary>
    [Section<SectionHeadlinePrimary>]
    [Scope<WWW.Classes._workspacekey_._classid_.Index>]
    [Cache]
    public sealed class ClassEditButtonFragment : FragmentControlButtonLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public ClassEditButtonFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = "kleenestar.core:class.edit.label";
            Icon = new IconPencil();
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = new PropertyColorButton(TypeColorButton.Primary);
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
                CoreHub.GetUri<WWW.Classes._workspacekey_._classid_.Edit>()
                    .BindParameters(renderContext.Request),
                TypeModalSize.ExtraLarge
                );

            return base.Render(renderContext, visualTree, Text, null, Tooltip, primaryAction, SecondaryAction, Icon);
        }
    }
}
