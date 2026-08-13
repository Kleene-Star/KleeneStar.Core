using System;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Core.WebRestApi
{
    /// <summary>
    /// Reads guid-valued fields out of a CRUD payload.
    /// </summary>
    /// <remarks>
    /// <see cref="RestApiCrudFormDataExtensions.BindTo"/> converts a payload value to the target
    /// property type through <c>Convert.ChangeType</c>, which cannot produce a <see cref="Guid"/>
    /// because <see cref="Guid"/> does not implement <c>IConvertible</c>; the resulting cast
    /// exception is swallowed, so a guid property — typically a foreign key such as a class or
    /// workspace reference — silently keeps its default value and the insert then fails on the
    /// foreign-key constraint. Until the framework converts guids itself, an endpoint binds such
    /// a property explicitly through this helper after calling <c>BindTo</c>.
    /// </remarks>
    public static class RestApiCrudFormDataGuidExtensions
    {
        /// <summary>
        /// Reads the guid stored under the given field name.
        /// </summary>
        /// <param name="fieldMap">The payload to read from.</param>
        /// <param name="name">
        /// The field name as declared on the entity; the lookup is case-insensitive, matching the
        /// lower-cased keys the payload parser produces.
        /// </param>
        /// <param name="value">
        /// When this method returns, contains the parsed guid, or <see cref="Guid.Empty"/> when the
        /// field is absent, empty or not a guid.
        /// </param>
        /// <returns>True when a guid could be read; otherwise false.</returns>
        public static bool TryGetGuid(this RestApiCrudFormData fieldMap, string name, out Guid value)
        {
            value = Guid.Empty;

            if (fieldMap is null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (!fieldMap.TryGetValue(name.ToLowerInvariant(), out var raw) || raw is null)
            {
                return false;
            }

            if (raw is Guid guid)
            {
                value = guid;

                return value != Guid.Empty;
            }

            return Guid.TryParse(raw.ToString(), out value) && value != Guid.Empty;
        }

        /// <summary>
        /// Reads an optional guid reference — a foreign key the payload may set, clear or leave
        /// alone.
        /// </summary>
        /// <remarks>
        /// A selection control submits the empty guid for its "none" entry, which means the caller
        /// cleared the reference, while an absent field means the form does not carry it at all
        /// and the stored value must stay. The return value distinguishes the two so a form that
        /// omits a reference cannot silently erase it.
        /// </remarks>
        /// <param name="fieldMap">The payload to read from.</param>
        /// <param name="name">The field name as declared on the entity; matched case-insensitively.</param>
        /// <param name="value">
        /// When this method returns true, contains the referenced id, or null when the payload
        /// clears the reference.
        /// </param>
        /// <returns>True when the payload carries the field at all; otherwise false.</returns>
        public static bool TryGetGuidReference(this RestApiCrudFormData fieldMap, string name, out Guid? value)
        {
            value = null;

            if (fieldMap is null || string.IsNullOrEmpty(name) || !fieldMap.ContainsKey(name.ToLowerInvariant()))
            {
                return false;
            }

            if (fieldMap.TryGetGuid(name, out var guid))
            {
                value = guid;
            }

            return true;
        }
    }
}
