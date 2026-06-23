using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WebUri
{
    /// <summary>
    /// Variable path segment that matches a saved-search id (a GUID).
    /// </summary>
    /// <typeparam name="TParameter">The parameter type.</typeparam>
    public class SavedSearchIdUriPathSegmentVariable<TParameter> : UriPathSegmentVariableGuid<TParameter>
        where TParameter : IParameterStatic, new()
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tag">The tag or null.</param>
        public SavedSearchIdUriPathSegmentVariable(object tag = null)
            : base(tag)
        {
        }

        /// <summary>
        /// Returns the variable.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The variable value pair.</returns>
        public override IDictionary<string, string> GetVariable(string value)
        {
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Make a deep copy.
        /// </summary>
        /// <returns>The copy.</returns>
        public override IUriPathSegment Copy()
        {
            return new SavedSearchIdUriPathSegmentVariable<TParameter>(this)
            {
                Expression = Expression,
                Value = Value,
                IsHidden = IsHidden,
                Uri = Uri
            };
        }

        /// <summary>
        /// Checks whether the node matches the path element.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the path element matched, false otherwise.</returns>
        public override bool IsMatched(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            else if (string.IsNullOrWhiteSpace(Expression) && Value.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (Regex.IsMatch(value, Expression, RegexOptions.IgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the display text for the saved-search id (its name).
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <returns>The saved-search name, or <see langword="null"/>.</returns>
        public override string GetDisplayText(IRenderContext renderContext)
        {
            var guid = Guid.TryParse(Value, out var id) ? id : Guid.Empty;

            return CoreHub.SavedSearchManager.GetSavedSearch(guid)?.Name;
        }

        /// <summary>
        /// Returns the icon for the saved-search id. Saved searches carry no icon, so this
        /// is always <see langword="null"/>.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <returns><see langword="null"/>.</returns>
        public override IIcon GetIcon(IRenderContext renderContext)
        {
            return null;
        }
    }
}
