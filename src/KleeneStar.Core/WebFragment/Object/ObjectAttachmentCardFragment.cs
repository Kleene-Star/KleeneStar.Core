using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Object-scoped content card that lists the files attached to the object currently
    /// displayed on <see cref="WWW.Issue._objectkey_.Index"/> and offers a drag-and-drop
    /// upload zone for adding new ones.
    /// </summary>
    /// <remarks>
    /// The card hosts a <see cref="ControlFileList"/> populated from
    /// <see cref="IAttachmentManager.GetAttachments(System.Guid)"/> (one
    /// <see cref="ControlFileListItem"/> per attachment, with a content-type-derived icon)
    /// followed by a <see cref="ControlUpload"/>. The upload posts back to the object's own
    /// page route; its <see cref="ControlUpload.Process(System.Action{ControlFormEventItemProcess{ControlFormInputValueFile}})"/>
    /// handler writes the binary payload below the application data directory and persists
    /// the attachment metadata through the manager.
    /// </remarks>
    [Section<SectionContentSecondary>]
    [Scope<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>]
    [Scope<global::KleeneStar.Core.WWW.Asset._objectkey_.Index>]
    [Order(0)]
    [Cache]
    public sealed class ObjectAttachmentCardFragment : FragmentControlPanel
    {
        private readonly IObjectManager _objectManager;
        private readonly IAttachmentManager _attachmentManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The fragment context.</param>
        /// <param name="objectManager">The object manager used to resolve the current
        /// object from the URL-bound object key.</param>
        /// <param name="attachmentManager">The attachment manager used to read and persist
        /// the object's files.</param>
        public ObjectAttachmentCardFragment(IFragmentContext fragmentContext, IObjectManager objectManager, IAttachmentManager attachmentManager)
            : base(fragmentContext)
        {
            _objectManager = objectManager;
            _attachmentManager = attachmentManager;
        }

        /// <summary>
        /// Renders the attachment card for the current object. Returns <c>null</c> when the
        /// fragment's render conditions exclude it or when no object can be resolved from
        /// the request.
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

            var card = new ControlPanelCard("object-attachments-card")
            {
                Header = _ => "kleenestar.core:object.attachment.card.header",
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two)
            };

            card.Add(BuildFileList(@object));
            card.Add(BuildUpload(@object, renderContext));

            return card.Render(renderContext, visualTree);
        }

        /// <summary>
        /// Builds the file list of the object's existing attachments. One
        /// <see cref="ControlFileListItem"/> is produced per attachment; the icon is
        /// derived from the content type and the metadata (name, size, date, description)
        /// is taken straight from the entity.
        /// </summary>
        /// <param name="object">The object whose attachments are listed.</param>
        /// <returns>The populated file list control.</returns>
        private ControlFileList BuildFileList(Model.Entities.Object @object)
        {
            var fileList = new ControlFileList("object-attachment-list");

            var items = _attachmentManager.GetAttachments(@object.Id)
                .Select(a => new ControlFileListItem($"attachment-{a.Id}")
                {
                    Icon = _ => ResolveIcon(a.ContentType, a.FileName),
                    Name = _ => a.FileName,
                    Size = _ => a.Size,
                    Date = _ => a.Created,
                    Description = _ => a.Description,
                    Uri = _ => ResolveDownloadUri(a.Id)
                })
                .ToArray();

            fileList.Add(items);

            return fileList;
        }

        /// <summary>
        /// Builds the drag-and-drop upload control. The control posts back to the object's
        /// page route and persists every selected file through the attachment manager when
        /// the form is processed.
        /// </summary>
        /// <param name="object">The object the uploaded files are attached to.</param>
        /// <param name="renderContext">The current render context; used to bind the upload
        /// URI to the active request's route parameters.</param>
        /// <returns>The configured upload control.</returns>
        private ControlUpload BuildUpload(Model.Entities.Object @object, IRenderControlContext renderContext)
        {
            var uploadUri = ResolveUploadUri(renderContext);
            var objectId = @object.Id;

            var upload = new ControlUpload("object-attachment-upload")
            {
                Uri = _ => uploadUri,
                Multiple = _ => true,
                AutoUpload = _ => true,
                Placeholder = _ => "kleenestar.core:object.attachment.upload.placeholder",
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Three, PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.None)
            };

            upload.Process(eventArgument => PersistUpload(objectId, eventArgument.Value));

            return upload;
        }

        /// <summary>
        /// Persists a single uploaded file: writes the binary payload below the application
        /// data directory and stores the attachment metadata through the manager. Failures
        /// are swallowed so a single bad file does not break the page render.
        /// </summary>
        /// <param name="objectId">The id of the object the file is attached to.</param>
        /// <param name="value">The uploaded file value (name, content type and bytes).</param>
        private void PersistUpload(Guid objectId, ControlFormInputValueFile value)
        {
            if (value is null || string.IsNullOrWhiteSpace(value.Name))
            {
                return;
            }

            try
            {
                _attachmentManager.Add
                (
                    objectId,
                    value.Name,
                    value.ContentType.ToString(),
                    value.Data ?? [],
                    description: null,
                    uploaderId: null
                );
            }
            catch
            {
                // upload is best-effort per file; ignore failures from a single bad payload
            }
        }

        /// <summary>
        /// Returns the icon that best represents the supplied content type / file name.
        /// </summary>
        /// <param name="contentType">The MIME content type, or <c>null</c>.</param>
        /// <param name="fileName">The file name, used as a fallback when the content type is
        /// unspecified.</param>
        /// <returns>The icon to display next to the file.</returns>
        private static IIcon ResolveIcon(string contentType, string fileName)
        {
            var type = contentType?.ToLowerInvariant() ?? string.Empty;
            var name = fileName?.ToLowerInvariant() ?? string.Empty;

            if (type.StartsWith("image/"))
            {
                return new IconFileImage();
            }

            if (type == "application/pdf" || name.EndsWith(".pdf"))
            {
                return new IconFilePdf();
            }

            if (type.Contains("word") || name.EndsWith(".doc") || name.EndsWith(".docx"))
            {
                return new IconFileWord();
            }

            if (type.Contains("spreadsheet") || type.Contains("excel") || name.EndsWith(".xls") || name.EndsWith(".xlsx") || name.EndsWith(".csv"))
            {
                return new IconFileExcel();
            }

            if (type.Contains("zip") || name.EndsWith(".zip") || name.EndsWith(".7z") || name.EndsWith(".rar"))
            {
                return new IconFileZipper();
            }

            if (type.StartsWith("text/") || name.EndsWith(".txt") || name.EndsWith(".log"))
            {
                return new IconFileLines();
            }

            return new IconFile();
        }

        /// <summary>
        /// Builds the download URI for the supplied attachment: the binary download resource
        /// with the attachment id carried in the <c>id</c> query parameter.
        /// </summary>
        /// <param name="attachmentId">The id of the attachment to download.</param>
        /// <returns>The download URI, or <c>null</c> when the endpoint is not registered.</returns>
        private static IUri ResolveDownloadUri(Guid attachmentId)
        {
            return CoreHub.GetUri<global::KleeneStar.Core.WWW.Attachments.Download>()?
                .Add(new UriQuery("id", attachmentId.ToString()));
        }

        /// <summary>
        /// Resolves the URI the upload control posts to: the object's own page route bound
        /// to the active request so the object-key segment is preserved.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <returns>The bound upload URI, or <c>null</c> when the page is not registered.</returns>
        private static IUri ResolveUploadUri(IRenderControlContext renderContext)
        {
            var uri = CoreHub.GetUri<global::KleeneStar.Core.WWW.Issue._objectkey_.Index>();
            if (uri is null)
            {
                return null;
            }

            return renderContext?.Request is null
                ? uri
                : uri.BindParameters(renderContext.Request);
        }
    }
}
