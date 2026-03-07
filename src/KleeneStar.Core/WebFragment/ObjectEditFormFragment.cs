using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using WebExpress.WebApp.WebApiControl;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a edit form fragment for a object.
    /// </summary>
    [Title("kleenestar.core:object.add.title")]
    [Section<SectionContentPreferences>]
    [Scope<WWW.Object._objectkey_.Edit>]
    [Cache]
    public sealed class ObjectEditFormFragment : FragmentControlRestFormAdd
    {
        /// <summary>
        /// Returns the input text control for specifying the summary of the object.
        /// </summary>
        public ControlRestFormItemInputUnique Summary { get; } = new()
        {
            Name = nameof(Object.Summary),
            Label = "kleenestar.core:object.summary.label",
            Placeholder = "kleenestar.core:object.summary.placeholder",
            Help = "kleenestar.core:object.summary.help",
            Required = true,
            RestUri = CoreHub.GetUri<WWW.Api._1_.Workspaces.UniqueName>()
        };

        /// <summary>
        /// Returns the input text control for specifying the description of the object.
        /// </summary>
        public ControlFormItemInputText Description { get; } = new ControlFormItemInputText()
        {
            Name = nameof(Workspace.Description),
            Label = "kleenestar.core:object.description.label",
            Placeholder = "kleenestar.core:object.description.placeholder",
            Format = TypeEditTextFormat.Wysiwyg,
            Required = false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public ObjectEditFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Add(Summary);
            Add(Description);

            Mode = TypeRestFormMode.Edit;
            Uri = CoreHub.GetUri<WWW.Api._1_.Objects.Index>();
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
            var key = renderContext.Request.GetParameter<ObjectKeyParameter>();
            var id = CoreHub.ObjectManager.GetObjectByKey(key)?
                .Id.ToString();

            return base.Render(renderContext, visualTree, Items, id, Uri);
        }
    }
}
