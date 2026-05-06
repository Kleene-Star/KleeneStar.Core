using KleeneStar.Core.WebParameter;
using System;
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
    /// Represents a back button fragment for forms, providing navigation functionality to return 
    /// to the previous view within a form context.
    /// </summary>
    [Section<SectionHeadlinePrologue>]
    [Scope<global::KleeneStar.Core.WWW.Form._formid_.Index>]
    [Cache]
    public sealed class FormBackButtonFragment : FragmentControlButtonLink
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for its operation. 
        /// Cannot be null.
        /// </param>
        public FormBackButtonFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = _ => "kleenestar.core:form.back.label";
            Icon = _ => new IconArrowLeft();
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = _ => new PropertyColorButton(TypeColorButton.Secondary);
            Outline = _ => true;
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
            var formIdParameter = renderContext.Request.GetParameter<FormIdParameter>();
            var formId = Guid.TryParse(formIdParameter?.Value, out var result) ? result : Guid.Empty;
            var form = CoreHub.FormManager.GetForm(formId);

            var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Forms._classid_.Index>()
                .BindParameters(new ClassIdParameter(form.ClassId));

            return base.Render(renderContext, visualTree);
        }
    }
}
