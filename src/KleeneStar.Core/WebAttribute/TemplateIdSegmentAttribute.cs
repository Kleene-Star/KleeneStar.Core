using System;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebUri;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WebAttribute
{
    /// <summary>
    /// Specifies a template id for use in endpoint routing, associating a parameter type 
    /// with a variable name and display format for URI path segments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TemplateIdSegmentAttribute : Attribute, IEndpointAttribute, ISegmentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TemplateIdSegmentAttribute()
        {
        }

        /// <summary>
        /// Conversion to a path segment.
        /// </summary>
        /// <returns>The path segment.</returns>
        public IUriPathSegment ToPathSegment()
        {
            return new TemplateIdUriPathSegmentVariable<TemplateIdParameter>();
        }
    }
}
