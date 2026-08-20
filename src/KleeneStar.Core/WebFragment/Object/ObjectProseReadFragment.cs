using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using System;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Main-panel content of the prose reading views shared by the document and blog
    /// detail pages (<see cref="WWW.Document._objectkey_.Index"/> and
    /// <see cref="WWW.Blog._objectkey_.Index"/>): a clean, publication-style rendering of
    /// the object's rich-text <see cref="Model.Entities.Object.Description"/> body. The
    /// title is already shown by the page headline, so it is not repeated here. On a blog
    /// post the body is preceded by a small meta line carrying the creation date and the
    /// author.
    /// </summary>
    /// <remarks>
    /// This is a reading view, not an editor: the body is rendered verbatim as HTML (the
    /// description is authored in the WYSIWYG editor of the sibling edit page), without
    /// the inline smart-edit controls the issue detail uses. Editing is reached through
    /// <see cref="ObjectProseEditButtonFragment"/> in the headline.
    /// </remarks>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Document._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Blog._objectkey_.Index>]
    [Cache]
    public sealed class ObjectProseReadFragment : FragmentControlPanel
    {
        private readonly IObjectManager _objectManager;
        private readonly IIdentityManager _identityManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to resolve the addressed
        /// object from the URL-bound object key.</param>
        /// <param name="identityManager">The identity manager used to resolve the blog
        /// post's author display name.</param>
        public ObjectProseReadFragment(IFragmentContext fragmentContext, IObjectManager objectManager, IIdentityManager identityManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _identityManager = identityManager;
        }

        /// <summary>
        /// Renders the reading view. Returns <c>null</c> when the fragment's render
        /// conditions exclude it or when no object can be resolved from the request.
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

            var id = @object.Id.ToString("N");

            // the body of a document or a post is the page, not a thing on it - it carries no
            // frame and no section label, only the reading measure the stylesheet gives it
            var body = new ControlPanel("object-prose-" + id)
            {
                Classes = ["wx-kleenestar-object-prose"]
            };

            var isBlog = string.Equals(@object.Kind, Model.Entities.ObjectKind.Blog, StringComparison.OrdinalIgnoreCase);

            if (isBlog)
            {
                body.Add(new ControlText("object-prose-meta-" + id)
                {
                    Text = _ => BuildBlogMeta(@object),
                    Format = _ => TypeFormatText.Small
                });
            }

            if (string.IsNullOrWhiteSpace(@object.Description))
            {
                body.Add(new ControlText("object-prose-empty-" + id)
                {
                    Text = _ => isBlog
                        ? "kleenestar.core:object.kind.blog.read.empty"
                        : "kleenestar.core:object.kind.document.read.empty",
                    Format = _ => TypeFormatText.Paragraph
                });

                return body.Render(renderContext, visualTree);
            }

            body.Add(new ControlHtml("object-prose-body-" + id)
            {
                Html = _ => "<div class=\"wx-kleenestar-prose\">" + @object.Description + "</div>"
            });

            return body.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Builds the blog meta line: the creation date, optionally followed by the
        /// author display name resolved from <see cref="Model.Entities.Object.CreatorId"/>.
        /// </summary>
        /// <param name="object">The blog post whose meta line is built.</param>
        /// <returns>The meta text, e.g. <c>"2026-07-12 · Jane Doe"</c>.</returns>
        private string BuildBlogMeta(Model.Entities.Object @object)
        {
            var date = @object.Created.ToString("yyyy-MM-dd");

            var author = @object.CreatorId.HasValue
                ? _identityManager.GetIdentity(@object.CreatorId.Value)?.Name
                : null;

            return string.IsNullOrWhiteSpace(author) ? date : date + " · " + author;
        }
    }
}
