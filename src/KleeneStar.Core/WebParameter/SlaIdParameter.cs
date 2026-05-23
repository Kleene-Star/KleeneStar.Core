using System;
using WebExpress.WebCore.WebParameter;

namespace KleeneStar.Core.WebParameter
{
    /// <summary>
    /// Represents a parameter that specifies the id of an <see cref="Model.Entities.SlaPolicy"/>.
    /// </summary>
    public sealed class SlaIdParameter : IParameterStatic
    {
        /// <summary>
        /// Gets the key that uniquely identifies the parameter in configuration or
        /// settings contexts.
        /// </summary>
        public static string Key => "slaid";

        /// <summary>
        /// Gets or sets the scope of the parameter.
        /// </summary>
        public ParameterScope Scope { get; set; }

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SlaIdParameter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class with a specified string value.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        public SlaIdParameter(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the class with a specified GUID value.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        public SlaIdParameter(Guid value)
        {
            Value = value.ToString();
        }

        /// <summary>
        /// Retrieves the unique key associated with the current instance.
        /// </summary>
        /// <returns>The unique key.</returns>
        public string GetKey()
        {
            return Key;
        }
    }
}
