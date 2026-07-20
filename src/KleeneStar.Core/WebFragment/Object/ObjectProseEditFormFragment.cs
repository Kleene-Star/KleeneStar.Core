using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// The prose edit form shared by the document and blog edit pages
    /// (<see cref="WWW.Document._objectkey_.Edit"/> and
    /// <see cref="WWW.Blog._objectkey_.Edit"/>): a form that edits just the two prose
    /// attributes of the object — its <see cref="Model.Entities.Object.Summary"/> (title)
    /// and its rich-text <see cref="Model.Entities.Object.Description"/> (body). It is
    /// surfaced as the body of a fullscreen modal opened from the reading view by
    /// <see cref="ObjectProseEditButtonFragment"/>.
    /// </summary>
    /// <remarks>
    /// The form loads and submits through the object CRUD endpoint
    /// (<see cref="WWW.Api._1_.Objects.Index"/>); <see cref="ItemId"/> addresses the row
    /// by the object resolved from the URL-bound object key. Unlike the issue edit form
    /// (<see cref="ObjectEditFormFragment"/>) it never reproduces the class's dynamic
    /// field structure — documents and blogs are prose, not work items — so exactly the
    /// two system inputs are rendered. It lives in <see cref="SectionContentPreferences"/>
    /// (the section a modal renders), matching the issue edit form, and the enclosing
    /// modal closes automatically on a successful save.
    /// </remarks>
    [Section<SectionContentPreferences>]
    [Scope<global::KleeneStar.Core.WWW.Document._objectkey_.Edit>]
    [Scope<global::KleeneStar.Core.WWW.Blog._objectkey_.Edit>]
    [Cache]
    public sealed class ObjectProseEditFormFragment : FragmentControlDataFormEdit
    {
        /// <summary>
        /// Gets the input for the object's title (its summary).
        /// </summary>
        public ControlFormItemInputText Summary { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Object.Summary),
            Label = _ => "kleenestar.core:object.summary.label",
            Placeholder = _ => "kleenestar.core:object.summary.placeholder",
            Help = _ => "kleenestar.core:object.summary.help",
            Required = _ => true
        };

        /// <summary>
        /// Gets the input for the object's rich-text body (its description).
        /// </summary>
        public ControlFormItemInputText Description { get; } = new()
        {
            Name = _ => nameof(Model.Entities.Object.Description),
            Label = _ => "kleenestar.core:object.description.label",
            Placeholder = _ => "kleenestar.core:object.description.placeholder",
            Format = _ => TypeEditTextFormat.Wysiwyg,
            Required = _ => false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectProseEditFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            // the form's REST service is declared by the endpoint type so the client loads
            // and submits the object through the emitted wx-service island; ItemId
            // addresses the row in the body
            this.DataService<global::KleeneStar.Core.WWW.Api._1_.Objects.Index>();

            ItemId = renderContext =>
            {
                var objectKey = renderContext.Request.GetParameter<ObjectKeyParameter>();
                var @object = CoreHub.ObjectManager.GetObjectByKey(objectKey);

                return @object?.Id.ToString();
            };
        }

        /// <summary>
        /// Renders the form with its two system inputs.
        /// </summary>
        /// <param name="renderContext">The context in which the control is rendered.</param>
        /// <param name="visualTree">The visual tree representing the control's structure.</param>
        /// <returns>An HTML node representing the rendered control.</returns>
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree)
        {
            return base.Render(renderContext, visualTree, [Summary, Description]);
        }
    }
}
