using System;
using System.IO;
using System.Linq;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebResource;

namespace KleeneStar.Core.WWW.Assets.Icons
{
    /// <summary>
    /// Represents a resource handler that serves icon files from the application's data path. 
    /// </summary>
    [IncludeSubPaths(true)]
    public sealed class Index : IResource
    {
        private readonly IResourceContext _resourceContext;
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="resourceContext">The resource context.</param>
        public Index(IResourceContext resourceContext)
        {
            _resourceContext = resourceContext;

            _path = Path.Combine(_resourceContext.ApplicationContext.DataPath, "icons");
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        public IResponse Process(IRequest request)
        {
            var route = _resourceContext.Route;
            var filename = request.Uri.PathSegments.Skip(route.PathSegments.Count()).LastOrDefault()?.ToString();
            var mimeType = GetMimeType(filename);

            var data = File.ReadAllBytes(Path.Combine(_path, filename));

            var response = new ResponseOK();
            response.Header.ContentLength = data != null ? data.Length : 0;
            response.Header.ContentType = mimeType;
            response.Header.CacheControl = "public, max-age=31536000, immutable";

            response.Content = data;

            return response;
        }

        /// <summary>
        /// Returns the MIME type associated with the specified file name based on its extension.
        /// </summary>
        /// <param name="filename">
        /// The name of the file whose MIME type is to be determined. The file extension is used to infer the MIME type.
        /// </param>
        /// <returns>A string representing the MIME type corresponding to the file extension. Returns "application/octet-stream"
        /// if the extension is not recognized.</returns>
        private static string GetMimeType(string filename)
        {
            var extension = Path.GetExtension(filename)?.ToLowerInvariant();

            return extension switch
            {
                ".svg" => "image/svg+xml",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".ico" => "image/x-icon",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                ".tiff" => "image/tiff",
                _ => "application/octet-stream" // fallback
            };
        }

    }
}
