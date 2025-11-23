using WebExpress.WebCore.WebParameter;

namespace KleeneStar.Core.WebParameter
{
    /// <summary>
    /// Represents a parameter that specifies a workspace category.
    /// </summary>
    public class CategoryParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public CategoryParameter()
            : base("Category", null, ParameterScope.Url)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class with a specified value.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        public CategoryParameter(string value)
            : base("Category", value, ParameterScope.Url)
        {
        }
    }
}
