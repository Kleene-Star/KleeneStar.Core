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
    /// Represents a button link for adding a new identity.
    /// </summary>
    [Section<SectionHeadlinePrimary>]
    [Scope<global::KleeneStar.Core.WWW.Settings.Identities.Index>]
    [Cache]
    public sealed class IdentityAddButtonFragment : FragmentControlButtonLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public IdentityAddButtonFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = _ => "kleenestar.core:setting.identity.add.label";
            Icon = _ => new IconPlus(TypeIconTheme.Light);
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = _ => new PropertyColorButton(TypeColorButton.Primary);
        }

        /// <summary>
        /// Convert the fragment to HTML.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var primaryAction = new ActionModal
            (
                "modal-form",
                CoreHub.GetUri<global::KleeneStar.Core.WWW.Settings.Identities.Add>()
                    .BindParameters(renderContext.Request),
                TypeModalSize.ExtraLarge
            );

            return base.Render(renderContext, visualTree);
        }
    }
}
