using KleeneStar.Core.WebParameter;
using System;
using WebExpress.WebApp.WebControl;
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
    /// Represents a fragment control that displays the three fixed forms (create, edit, view) per
    /// class within a ControlView. This fragment is only rendered for standard forms. Additional
    /// forms do not have these predefined views and will display an empty or custom layout instead.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Form._formid_.Index>]
    [Cache]
    public sealed class FormDetailViewAdditionalFragment : FragmentControlView
    {
        private const string FieldTableTemplateId = "tab-form-fields";

        /// <summary>
        /// Gets the REST tab control for the default form.
        /// </summary>
        public ControlRestTab DefaultTab { get; } = new ControlRestTab();

        /// <summary>
        /// Gets the REST table control for form field elements within the default tab.
        /// </summary>
        public ControlRestTable DefaultFieldTable { get; } = new ControlRestTable();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public FormDetailViewAdditionalFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            DefaultTab.Add(new ControlRestTabTemplate()
            {
                Id = FieldTableTemplateId
            }.Add(DefaultFieldTable));

            Add(new ControlViewItem()
            {
                Title = "kleenestar.core:form.default.label",
                Icon = new IconRectangleList()
            }.Add(DefaultTab));
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
            var formId = Guid.TryParse(formIdParam?.Value, out var id) ? id : Guid.Empty;

            // only render the three predefined views for additional forms
            if (CoreHub.FormManager.IsStandardForm(formId))
            {
                return null;
            }

            var tabUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Forms.Form._formid_.Tab>()?
                .BindParameters(formIdParam)
                .BindParameters(renderContext.Request);

            var tableUri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Forms.Form._formid_.Table>()?
                .BindParameters(formIdParam)
                .BindParameters(renderContext.Request);

            DefaultTab.RestUri = tabUri;
            DefaultFieldTable.RestUri = tableUri;

            return base.Render(renderContext, visualTree);
        }
    }
}
