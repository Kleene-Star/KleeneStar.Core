using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WebUri
{
    /// <summary>
    /// Variable path segment.
    /// </summary>
    /// <typeparam name="TParameter">The parameter type.</typeparam>
    public class WorkspaceKeyUriPathSegmentVariable<TParameter> : UriPathSegmentVariableRegex<TParameter>
        where TParameter : IParameterStatic, new()
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tag">The tag or null</param>
        public WorkspaceKeyUriPathSegmentVariable(object tag = null)
            : base(@"^[a-z0-9-]{1,10}$", tag)
        {
        }

        /// <summary>
        /// Make a deep copy.
        /// </summary>
        /// <returns>The copy.</returns>
        public override IUriPathSegment Copy()
        {
            return new WorkspaceKeyUriPathSegmentVariable<TParameter>()
            {
                Expression = Expression,
                Value = Value,
                IsHidden = IsHidden,
                Uri = Uri
            };
        }

        /// <summary>
        /// Returns a string that represents the display text for the current instance.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <returns>
        /// A string containing the display text associated with the instance. The 
        /// value may be empty if no display text is available.
        /// </returns>
        public override string GetDisplayText(IRenderContext renderContext)
        {
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(Value);

            return workspace?.Name;
        }

        /// <summary>
        /// Returns an icon that visually represents the parameter within the given render context.
        /// </summary>
        /// <param name="renderContext">
        /// The rendering context that provides information required to determine the appropriate icon.
        /// </param>
        /// <returns>
        /// An icon associated with the current instance. The value may be <c>null</c> or empty 
        /// if no icon is available.
        /// </returns>
        public override IIcon GetIcon(IRenderContext renderContext)
        {
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(Value);

            return workspace?.Icon;
        }
    }
}