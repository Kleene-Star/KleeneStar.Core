using System;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebParameter;

namespace KleeneStar.Core.WebParameter
{
    /// <summary>
    /// Represents a parameter that specifies a class id.
    /// </summary>
    public class ClassParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ClassParameter()
            : base("Class", null, ParameterScope.Url)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class with a specified value.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        public ClassParameter(string value)
            : base("class", value, ParameterScope.Url)
        {
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
            var guid = Guid.TryParse(Value, out var id) ? id : Guid.Empty;
            var @class = CoreHub.ClassManager.GetClass(guid);

            return @class?.Name;
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
            var guid = Guid.TryParse(Value, out var id) ? id : Guid.Empty;
            var @class = CoreHub.ClassManager.GetClass(guid);

            return @class.Icon;
        }
    }
}
