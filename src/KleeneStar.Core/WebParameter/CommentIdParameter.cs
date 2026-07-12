using System;
using WebExpress.WebCore.WebParameter;

namespace KleeneStar.Core.WebParameter
{
    /// <summary>
    /// Represents a parameter that specifies the id of a <see cref="Model.Entities.Comment"/>.
    /// </summary>
    public sealed class CommentIdParameter : IParameterStatic
    {
        /// <summary>
        /// Gets the unique key for this parameter.
        /// </summary>
        public static string Key => "commentid";

        /// <summary>
        /// Gets or sets the parameter scope.
        /// </summary>
        public ParameterScope Scope { get; set; }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with no value.
        /// </summary>
        public CommentIdParameter()
        {
        }

        /// <summary>
        /// Initializes a new instance with the supplied string value.
        /// </summary>
        /// <param name="value">The value.</param>
        public CommentIdParameter(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance with the supplied GUID value.
        /// </summary>
        /// <param name="value">The value.</param>
        public CommentIdParameter(Guid value)
        {
            Value = value.ToString();
        }

        /// <summary>
        /// Returns the unique key for this parameter (used by WebExpress to identify
        /// the parameter in request bindings).
        /// </summary>
        /// <returns>The key.</returns>
        public string GetKey()
        {
            return Key;
        }
    }
}
