using KleeneStar.Core.WebParameter;
using System;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Form
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
        private static readonly IUri _uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Forms._classid_.Index>();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">
        /// The context associated with the fragment, providing necessary data and services for 
        /// its operation. Cannot be null.
        /// </param>
        public FormBackButtonFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = _ => "kleenestar.core:form.back.label";
            Icon = _ => new IconArrowLeft();
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = _ => new PropertyColorButton(TypeColorButton.Secondary);
            Outline = _ => true;
            Uri = renderContext => GetUri(renderContext);
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

            return base.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Retrieves the URI for the index page of a form based on the current render context.
        /// </summary>
        /// <param name="renderContext">
        /// The context for the current render operation, providing access to request parameters and
        /// rendering state.
        /// </param>
        /// <returns>
        /// An object representing the URI for the form's index page, with parameters bound according 
        /// to the current context.
        /// </returns>
        private static IUri GetUri(IRenderControlContext renderContext)
        {
            var formIdParameter = renderContext.Request.GetParameter<FormIdParameter>();
            var formId = Guid.TryParse(formIdParameter?.Value, out var result) ? result : Guid.Empty;
            var form = CoreHub.FormManager.GetForm(formId);

            return _uri.BindParameters(new ClassIdParameter(form.ClassId));
        }
    }
}
