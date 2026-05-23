using KleeneStar.Core.WebManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WebUri
{
    /// <summary>
    /// Variable path segment carrying a <see cref="Model.Entities.Calendar"/> id.
    /// </summary>
    /// <typeparam name="TParameter">The parameter type.</typeparam>
    public class CalendarIdUriPathSegmentVariable<TParameter> : UriPathSegmentVariableGuid<TParameter>
        where TParameter : IParameterStatic, new()
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tag">Optional tag.</param>
        public CalendarIdUriPathSegmentVariable(object tag = null)
            : base(tag)
        {
        }

        /// <summary>
        /// Returns the variable key/value pairs contributed by this segment for the
        /// supplied raw path value. Calendar ids do not produce additional variables
        /// beyond the bound parameter, so the dictionary is empty.
        /// </summary>
        /// <param name="value">The raw path value as it appears in the URL.</param>
        /// <returns>An empty dictionary.</returns>
        public override IDictionary<string, string> GetVariable(string value)
        {
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Returns a deep copy of this segment, preserving expression, value, hidden
        /// flag, and target URI. Required by the routing engine when cloning segments
        /// between sitemap nodes.
        /// </summary>
        /// <returns>A new, independent segment instance.</returns>
        public override IUriPathSegment Copy()
        {
            return new CalendarIdUriPathSegmentVariable<TParameter>(this)
            {
                Expression = Expression,
                Value = Value,
                IsHidden = IsHidden,
                Uri = Uri
            };
        }

        /// <summary>
        /// Determines whether the supplied raw path value is a candidate calendar id
        /// for this segment. A value is rejected when it is empty or matches one of
        /// the <see cref="CalendarManager.ReservedCalendarNames"/>; otherwise the
        /// configured regular expression (or the literal value, when no expression is
        /// set) decides the match.
        /// </summary>
        /// <param name="value">The raw path value to check.</param>
        /// <returns><c>true</c> when the value matches; <c>false</c> otherwise.</returns>
        public override bool IsMatched(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            else if (CalendarManager.ReservedCalendarNames.Contains(value?.Trim().ToLower()))
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
        /// Returns the display text shown for this segment in breadcrumbs and other UI
        /// surfaces. Calendar ids are opaque GUIDs; the page that hosts the segment is
        /// expected to substitute a meaningful label, so this implementation returns
        /// <c>null</c>.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <returns><c>null</c>.</returns>
        public override string GetDisplayText(IRenderContext renderContext)
        {
            return null;
        }

        /// <summary>
        /// Returns the icon shown alongside this segment in breadcrumbs and other UI
        /// surfaces. Calendar segments do not contribute an icon of their own — the
        /// hosting page supplies one — so this implementation returns <c>null</c>.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <returns><c>null</c>.</returns>
        public override IIcon GetIcon(IRenderContext renderContext)
        {
            return null;
        }
    }
}
