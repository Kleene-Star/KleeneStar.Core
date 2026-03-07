using KleeneStar.Core.WebUri;
using System;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WebAttribute
{
    /// <summary>
    /// Specifies a workspace key for use in endpoint routing, associating a parameter type 
    /// with a variable name and display format for URI path segments.
    /// </summary>
    /// <typeparam name="TParameter">
    /// The type of parameter to associate with the segment key.
    /// </typeparam>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClassSegmentAttribute<TParameter> : Attribute, IEndpointAttribute, ISegmentAttribute
        where TParameter : IParameterStatic, new()
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ClassSegmentAttribute()
        {
        }

        /// <summary>
        /// Conversion to a path segment.
        /// </summary>
        /// <returns>The path segment.</returns>
        public IUriPathSegment ToPathSegment()
        {
            return new ClassIdUriPathSegmentVariable<TParameter>();
        }
    }
}
