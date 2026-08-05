using System;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Quickfilter
{
    /// <summary>
    /// Represents the form in which a user defines a new quickfilter for the bar the dialog was
    /// opened from.
    /// </summary>
    /// <remarks>
    /// Which bar the filter belongs to is not something the user picks: it is taken from the address
    /// the chip opened and carried in hidden fields, so the same dialog serves every view.
    /// </remarks>
    [Title("kleenestar.core:quickfilter.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Quickfilters.Add>]
    [Cache]
    public sealed class QuickfilterAddFormFragment : FragmentControlDataFormAdd
    {
        /// <summary>
        /// Gets the input control for the chip label.
        /// </summary>
        public ControlFormItemInputText QuickfilterName { get; } = new()
        {
            Name = _ => nameof(Model.Entities.CustomQuickfilter.Name),
            Label = _ => "kleenestar.core:quickfilter.name.label",
            Placeholder = _ => "kleenestar.core:quickfilter.name.placeholder",
            Help = _ => "kleenestar.core:quickfilter.name.help",
            Required = _ => true,
            MaxLength = _ => 256
        };

        /// <summary>
        /// Gets the input control for the filter expression.
        /// </summary>
        /// <remarks>
        /// The expression is the same WQL the view's advanced query accepts, so it can be tried out
        /// in the search bar before it is stored here.
        /// </remarks>
        public ControlFormItemInputText Query { get; } = new()
        {
            Name = _ => nameof(Model.Entities.CustomQuickfilter.Query),
            Label = _ => "kleenestar.core:quickfilter.query.label",
            Placeholder = _ => "kleenestar.core:quickfilter.query.placeholder",
            Help = _ => "kleenestar.core:quickfilter.query.help",
            Required = _ => true
        };

        /// <summary>
        /// Gets the toggle that offers the filter to everyone rather than to its owner alone.
        /// </summary>
        public ControlFormItemInputCheck Shared { get; } = new()
        {
            Name = _ => nameof(Model.Entities.CustomQuickfilter.Shared),
            Label = _ => "kleenestar.core:quickfilter.shared.label",
            Help = _ => "kleenestar.core:quickfilter.shared.help",
            Layout = _ => TypeLayoutCheck.Switch
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <remarks>
        /// The bar the filter is destined for travels on the address the chip opened, and is
        /// carried over to the address the form submits to rather than being put into the form as
        /// hidden fields: a data form leaves its items to the client, which never fills a hidden
        /// field the user cannot edit, and the destination is not the user's to choose anyway.
        /// </remarks>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public QuickfilterAddFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(QuickfilterName);
            Add(Query);
            Add(Shared);

            this.DataService<global::KleeneStar.Core.WWW.Api._1_.Quickfilters.Index>();

            // the preset is kept as the framework builds it, including the domain it announces on,
            // and only its address is extended
            var preset = ServiceFactories[0];
            ServiceFactories[0] = renderContext =>
            {
                var descriptor = preset(renderContext);
                descriptor.BaseUri = AppendScope(descriptor.BaseUri, renderContext);

                return descriptor;
            };
        }

        /// <summary>
        /// Carries the view and the context the dialog was opened for over to an address.
        /// </summary>
        /// <param name="uri">The address to extend.</param>
        /// <param name="renderContext">The context the dialog is rendered in.</param>
        /// <returns>The address with the destination of the new filter appended.</returns>
        private static string AppendScope(string uri, IRenderControlContext renderContext)
        {
            var view = renderContext?.Request?.GetParameter("view")?.Value;
            var context = renderContext?.Request?.GetParameter("context")?.Value;

            if (string.IsNullOrWhiteSpace(view))
            {
                return uri;
            }

            var separator = uri?.Contains('?') == true ? "&" : "?";
            var scope = $"{separator}view={System.Uri.EscapeDataString(view)}";

            return string.IsNullOrWhiteSpace(context)
                ? uri + scope
                : uri + scope + $"&context={System.Uri.EscapeDataString(context)}";
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
