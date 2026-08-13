using KleeneStar.Core.WebAttribute;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebRestApi;

namespace KleeneStar.Core.WWW.Api._1_.Templates._templateid_
{
    /// <summary>
    /// Provides a index for a single template. Declaring the template-id segment here is what
    /// turns the folder into a route variable, so the sibling endpoints receive the addressed
    /// template instead of a literal path segment.
    /// </summary>
    [TemplateIdSegment]
    [Cache]
    public sealed class Index : IRestApi
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }
    }
}
