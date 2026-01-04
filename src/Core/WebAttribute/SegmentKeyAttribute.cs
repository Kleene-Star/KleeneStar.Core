using KleeneStar.Core.WebParameter.Workspace;
using KleeneStar.Core.WebUri.Workspace;
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
    public class SegmentKeyAttribute<TParameter> : Attribute, IEndpointAttribute, ISegmentAttribute
        where TParameter : KeyParameter
    {
        /// <summary>
        /// Returns or sets the name of the variable.
        /// </summary>
        private string VariableName { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="display">The display string.</param>
        public SegmentKeyAttribute()
        {
            VariableName = (Activator.CreateInstance<TParameter>() as Parameter)?.Key?.ToLower();
        }

        /// <summary>
        /// Conversion to a path segment.
        /// </summary>
        /// <returns>The path segment.</returns>
        public IUriPathSegment ToPathSegment()
        {
            return new WorkspaceKeyUriPathSegmentVariable<TParameter>(VariableName);
        }
    }
}
