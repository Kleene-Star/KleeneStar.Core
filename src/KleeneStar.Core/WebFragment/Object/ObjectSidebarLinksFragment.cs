using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Sidebar group that lists every <see cref="ObjectLink"/> in which the current
    /// object participates. Each row shows the relation type, the partner object's
    /// key, and a one-line summary.
    /// </summary>
    /// <remarks>
    /// The fragment self-suppresses when no link exists for the current object.
    /// The relation label is rendered from the current object's perspective: when the
    /// current object is the source of the link the configured relation type is used
    /// directly; when it is the target the inverse relation is shown
    /// (e.g. <see cref="ObjectLinkRelationType.Blocks"/> instead of
    /// <see cref="ObjectLinkRelationType.BlockedBy"/>).
    /// </remarks>
    [Section<SectionSidebarPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Object._objectkey_.Index>]
    [Order(32)]
    [Cache]
    public sealed class ObjectSidebarLinksFragment : FragmentControlSidebarItemDynamic
    {
        private readonly IObjectManager _objectManager;
        private readonly IObjectLinkManager _linkManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObjectSidebarLinksFragment
        (
            IFragmentContext fragmentContext,
            IObjectManager objectManager,
            IObjectLinkManager linkManager
        )
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _linkManager = linkManager;

            Icon = _ => new IconLink();
            Tooltip = _ => "kleenestar.core:object.sidebar.links.label";

            RenderControl = (renderContext, visualTree) => RenderLinks(renderContext);
        }

        /// <summary>
        /// Renders the group, suppressing it when the current object has no links.
        /// </summary>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (!HasLinks(renderContext))
            {
                return null;
            }

            return base.Render(renderContext, visualTree);
        }

        private bool HasLinks(IRenderControlContext renderContext)
        {
            var current = ResolveCurrent(renderContext);
            return current is not null && _linkManager.GetLinks(current.Id).Any();
        }

        private Model.Entities.Object ResolveCurrent(IRenderControlContext renderContext)
        {
            var keyParameter = renderContext?.Request?.GetParameter<ObjectKeyParameter>();
            return _objectManager.GetObjectByKey(keyParameter?.Value);
        }

        private IHtmlNode RenderLinks(IRenderControlContext renderContext)
        {
            var current = ResolveCurrent(renderContext);
            var wrapper = new HtmlElementTextContentDiv()
            {
                Class = "wx-kleenestar-object-sidebar-group"
            };

            wrapper.Add(new HtmlElementTextContentDiv()
            {
                Class = "wx-kleenestar-object-sidebar-group__title"
            }.Add(new HtmlText(I18N.Translate(renderContext, "kleenestar.core:object.sidebar.links.label"))));

            if (current is null)
            {
                return wrapper;
            }

            foreach (var (partner, relation) in EnumerateLinks(current))
            {
                if (partner is null)
                {
                    continue;
                }

                var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Object._objectkey_.Index>()
                    .BindParameters(new ObjectKeyParameter(partner.Key))?
                    .ToString();

                var row = new HtmlElementTextSemanticsA()
                {
                    Class = "wx-kleenestar-object-sidebar-group__item",
                    Href = uri,
                    Title = partner.Summary
                };

                row.Add(new HtmlElementTextSemanticsSpan(new HtmlText(I18N.Translate(renderContext, relation.TranslationKey())))
                {
                    Class = "wx-kleenestar-object-sidebar-group__rel"
                });

                row.Add(new HtmlElementTextSemanticsSpan(new HtmlText(partner.Key))
                {
                    Class = "wx-kleenestar-object-sidebar-group__key"
                });

                row.Add(new HtmlElementTextSemanticsSpan(new HtmlText(partner.Summary))
                {
                    Class = "wx-kleenestar-object-sidebar-group__sum"
                });

                wrapper.Add(row);
            }

            return wrapper;
        }

        private IEnumerable<(Model.Entities.Object Partner, ObjectLinkRelationType Relation)> EnumerateLinks(Model.Entities.Object current)
        {
            foreach (var link in _linkManager.GetLinks(current.Id))
            {
                if (link.SourceObjectId == current.Id)
                {
                    yield return (link.TargetObject, link.RelationType);
                }
                else
                {
                    yield return (link.SourceObject, link.RelationType.Inverse());
                }
            }
        }
    }

    /// <summary>
    /// Provides translation-key lookups and inverse-relation resolution for
    /// <see cref="ObjectLinkRelationType"/>.
    /// </summary>
    internal static class ObjectLinkRelationTypeExtensions
    {
        /// <summary>
        /// Returns the i18n key for the relation type when shown on the source side.
        /// </summary>
        public static string TranslationKey(this ObjectLinkRelationType type)
        {
            return type switch
            {
                ObjectLinkRelationType.RelatesTo => "kleenestar.core:objectlink.relationtype.relatesto.label",
                ObjectLinkRelationType.BlockedBy => "kleenestar.core:objectlink.relationtype.blockedby.label",
                ObjectLinkRelationType.Blocks => "kleenestar.core:objectlink.relationtype.blocks.label",
                ObjectLinkRelationType.DuplicateOf => "kleenestar.core:objectlink.relationtype.duplicateof.label",
                ObjectLinkRelationType.CausedBy => "kleenestar.core:objectlink.relationtype.causedby.label",
                ObjectLinkRelationType.Causes => "kleenestar.core:objectlink.relationtype.causes.label",
                ObjectLinkRelationType.PartOf => "kleenestar.core:objectlink.relationtype.partof.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the inverse relation. Used when the current object is the link's
        /// target, so the displayed verb reflects the partner's role from the current
        /// object's perspective.
        /// </summary>
        public static ObjectLinkRelationType Inverse(this ObjectLinkRelationType type)
        {
            return type switch
            {
                ObjectLinkRelationType.BlockedBy => ObjectLinkRelationType.Blocks,
                ObjectLinkRelationType.Blocks => ObjectLinkRelationType.BlockedBy,
                ObjectLinkRelationType.CausedBy => ObjectLinkRelationType.Causes,
                ObjectLinkRelationType.Causes => ObjectLinkRelationType.CausedBy,
                _ => type
            };
        }
    }
}
