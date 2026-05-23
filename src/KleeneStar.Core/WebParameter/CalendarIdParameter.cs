using System;
using WebExpress.WebCore.WebParameter;

namespace KleeneStar.Core.WebParameter
{
    /// <summary>
    /// Represents a parameter that specifies the id of a <see cref="Model.Entities.Calendar"/>.
    /// </summary>
    public sealed class CalendarIdParameter : IParameterStatic
    {
        /// <summary>
        /// Gets the unique key for this parameter.
        /// </summary>
        public static string Key => "calendarid";

        /// <summary>
        /// Gets or sets the parameter scope.
        /// </summary>
        public ParameterScope Scope { get; set; }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public CalendarIdParameter()
        {
        }

        /// <summary>
        /// Initializes a new instance with a string value.
        /// </summary>
        /// <param name="value">The value.</param>
        public CalendarIdParameter(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance with a GUID value.
        /// </summary>
        /// <param name="value">The value.</param>
        public CalendarIdParameter(Guid value)
        {
            Value = value.ToString();
        }

        /// <summary>
        /// Returns the unique key for this parameter.
        /// </summary>
        /// <returns>The key.</returns>
        public string GetKey()
        {
            return Key;
        }
    }
}
