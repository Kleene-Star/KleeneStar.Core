using KleeneStar.Core.WebManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.WebUri.Workspace
{
    /// <summary>
    /// Variable path segment.
    /// </summary>
    /// <typeparam name="TParameter">The parameter type.</typeparam>
    public class WorkspaceKeyUriPathSegmentVariable<TParameter> : UriPathSegmentVariable<TParameter>
        where TParameter : IParameter
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tag">The tag or null</param>
        public WorkspaceKeyUriPathSegmentVariable(string name, object tag = null)
            : base(name, tag)
        {
            VariableName = name;
            Value = name;
            Expression = @"^[a-z-0-9]{1,10}$";
            Tag = tag;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="segment">The path segment to copy.</param>
        public WorkspaceKeyUriPathSegmentVariable(WorkspaceKeyUriPathSegmentVariable<TParameter> segment)
            : base(segment.VariableName, segment.Tag)
        {
            Expression = segment.Expression;
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
            return new WorkspaceKeyUriPathSegmentVariable<TParameter>(this)
            {
                Value = Value
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
            else if (WorkspaceManager.ReservedWorkspaceKeys.Contains(value?.Trim().ToLower()))
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
    }
}