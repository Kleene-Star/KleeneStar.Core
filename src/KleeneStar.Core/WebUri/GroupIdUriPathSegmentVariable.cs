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
    /// Variable path segment for group id.
    /// </summary>
    /// <typeparam name="TParameter">The parameter type.</typeparam>
    public class GroupIdUriPathSegmentVariable<TParameter> : UriPathSegmentVariableGuid<TParameter>
        where TParameter : IParameterStatic, new()
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tag">The tag or null</param>
        public GroupIdUriPathSegmentVariable(object tag = null)
            : base(tag)
        {
        }

        /// <summary>
        /// Returns the variable.
        /// </summary>
        public override IDictionary<string, string> GetVariable(string value)
        {
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Make a deep copy.
        /// </summary>
        public override IUriPathSegment Copy()
        {
            return new GroupIdUriPathSegmentVariable<TParameter>(this)
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
        public override bool IsMatched(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            else if (GroupManager.ReservedGroupNames.Contains(value?.Trim().ToLower()))
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
        /// Returns the display text.
        /// </summary>
        public override string GetDisplayText(IRenderContext renderContext)
        {
            return null;
        }

        /// <summary>
        /// Returns the icon.
        /// </summary>
        public override IIcon GetIcon(IRenderContext renderContext)
        {
            return null;
        }
    }
}
