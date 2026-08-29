using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebRestApi;
using System.Globalization;
using System.Linq;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// The relations of the reduced object view: what the object is connected to, listed for
    /// reading in the detail pane a list row opens.
    /// </summary>
    /// <remarks>
    /// The pane shows the same relations as the full view and reads them from the same
    /// endpoint, but it is read-only. A pane a few hundred pixels wide is where somebody
    /// checks what an object is connected to while working through a list; establishing a
    /// relation means picking a target, a type and a note, which is a task for the full view -
    /// and <see cref="ObjectPreviewOpenFragment"/> is the button that gets there. Suppressing
    /// the add affordance here also keeps the modal dialog out of a frame it would overflow.
    /// <para>
    /// The graph view stays available: it costs no round trip, is derived from the relations
    /// already loaded, and is the reading a narrow column actually benefits from.
    /// </para>
    /// </remarks>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Preview>]
    [Order(3)]
    [Cache]
    public sealed class ObjectPreviewRelationFragment : FragmentControlPanel
    {
        private readonly IObjectManager _objectManager;
        private readonly IObjectRelationManager _relationManager;

        /// <summary>
        /// Gets the REST-backed relation surface, rendered for reading only.
        /// </summary>
        public ControlDataRelationView Relations { get; } = new("object-preview-relations")
        {
            Subject = renderContext => renderContext?.Request?.GetParameter<ObjectKeyParameter>()?.Value,
            SubjectClass = renderContext => ResolveClassName(renderContext),
            Layout = _ => TypeLayoutRelationView.Flat,
            Readonly = _ => true,

            // the enclosing section already carries the icon, the caption and the count; in a
            // pane this narrow a repeated header costs a row that has to earn its height
            HeaderIcon = _ => false,
            HeaderText = _ => false,
            HeaderBadge = _ => false
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to resolve the current object
        /// from the URL-bound object key.</param>
        /// <param name="relationManager">The relation manager, read for the count the header
        /// reports.</param>
        public ObjectPreviewRelationFragment(IFragmentContext fragmentContext, IObjectManager objectManager, IObjectRelationManager relationManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _relationManager = relationManager;

            // the targets service is deliberately absent: a read-only surface opens no add
            // dialog, so nothing would ever ask it
            Relations
                .DataService<global::KleeneStar.Core.WWW.Api._1_.Relations._objectkey_.Index>()
                .SystemsService<global::KleeneStar.Core.WWW.Api._1_.Relations.Systems>();
        }

        /// <summary>
        /// Renders the relations of the previewed object. Returns <c>null</c> when the
        /// fragment's render conditions exclude it, when no object can be resolved, or when
        /// the object holds no relations - an empty state would take a third of the pane to
        /// say nothing.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        /// <returns>The HTML node, or <c>null</c>.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!FragmentContext.Conditions.Check(renderContext?.Request))
            {
                return null;
            }

            var keyParameter = renderContext?.Request?.GetParameter<ObjectKeyParameter>();
            var @object = _objectManager.GetObjectByKey(keyParameter?.Value);

            if (@object is null)
            {
                return null;
            }

            var count = _relationManager.GetRelations(@object.Id).Count();

            if (count == 0)
            {
                return null;
            }

            var section = new ControlSection("object-preview-relations-section")
            {
                Header = _ => "kleenestar.core:object.relations.card.header",
                HeaderIcon = _ => new IconLink(),
                Layout = _ => TypeLayoutSection.Rule,
                Badge = _ => count.ToString(CultureInfo.InvariantCulture)
            };

            section.Add(Relations);

            return section.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Resolves the class name of the object the route addresses.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <returns>The class name, or <c>null</c>.</returns>
        private static string ResolveClassName(IRenderControlContext renderContext)
        {
            var key = renderContext?.Request?.GetParameter<ObjectKeyParameter>()?.Value;

            return ObjectRelationProjection.ClassNameOf(CoreHub.ObjectManager.GetObjectByKey(key));
        }
    }
}
