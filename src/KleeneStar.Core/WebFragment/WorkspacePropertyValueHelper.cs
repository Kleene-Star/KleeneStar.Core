using KleeneStar.Model.Entities;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace KleeneStar.Core.WebFragment
{
    internal static class WorkspacePropertyValueHelper
    {
        public static string JoinEnumerable(object value)
        {
            if (value is string text)
            {
                return text;
            }

            if (value is not IEnumerable enumerable)
            {
                return string.Empty;
            }

            return string.Join(", ", enumerable.Cast<object>().Select(x => x?.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        public static bool ReadBoolean(Workspace workspace, string propertyName)
        {
            var value = workspace?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)?.GetValue(workspace);
            return value is bool b && b;
        }

        public static string ReadString(Workspace workspace, string propertyName)
        {
            return workspace?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)?.GetValue(workspace)?.ToString();
        }

        public static object ReadValue(Workspace workspace, string propertyName)
        {
            return workspace?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)?.GetValue(workspace);
        }

        public static Guid? ReadGuid(Workspace workspace, string propertyName)
        {
            var value = ReadValue(workspace, propertyName);
            return value switch
            {
                Guid guid when guid != Guid.Empty => guid,
                Guid? guid when guid.HasValue && guid.Value != Guid.Empty => guid,
                _ => null
            };
        }
    }
}
