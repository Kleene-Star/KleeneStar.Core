using KleeneStar.Core.WebParameter;
using KleeneStar.Core.WebUri;
using System;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WebAttribute
{
    /// <summary>
    /// Specifies a calendar id segment for endpoint routing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CalendarIdSegmentAttribute : Attribute, IEndpointAttribute, ISegmentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public CalendarIdSegmentAttribute()
        {
        }

        /// <summary>
        /// Conversion to a path segment.
        /// </summary>
        /// <returns>The path segment.</returns>
        public IUriPathSegment ToPathSegment()
        {
            return new CalendarIdUriPathSegmentVariable<CalendarIdParameter>();
        }
    }
}
