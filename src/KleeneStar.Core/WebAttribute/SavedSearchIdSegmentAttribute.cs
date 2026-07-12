using System;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebUri;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WebAttribute
{
    /// <summary>
    /// Specifies a saved-search id for use in endpoint routing, associating the
    /// <see cref="SavedSearchIdParameter"/> with a variable URI path segment.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SavedSearchIdSegmentAttribute : Attribute, IEndpointAttribute, ISegmentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SavedSearchIdSegmentAttribute()
        {
        }

        /// <summary>
        /// Conversion to a path segment.
        /// </summary>
        /// <returns>The path segment.</returns>
        public IUriPathSegment ToPathSegment()
        {
            return new SavedSearchIdUriPathSegmentVariable<SavedSearchIdParameter>();
        }
    }
}
