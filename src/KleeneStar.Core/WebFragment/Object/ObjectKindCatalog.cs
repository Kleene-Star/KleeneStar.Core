using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// The registry of the object kinds (subtypes) known to the application. The core
    /// registers its built-in kinds (document, blog, issue); add-ons extend the set by
    /// calling <see cref="Register"/> with their own <see cref="IObjectKind"/>
    /// descriptor, typically from their plugin initialization.
    /// </summary>
    /// <remarks>
    /// The catalog is the semantic lookup behind the persisted
    /// <see cref="Model.Entities.Object.Kind"/> key. It deliberately does not gate
    /// persistence — unknown keys survive in the data layer so objects of an add-on
    /// kind outlive the add-on — but UI components use the catalog to resolve a key
    /// to its presentation (label, icon, overview page).
    /// </remarks>
    public static class ObjectKindCatalog
    {
        private static readonly object _sync = new();
        private static readonly Dictionary<string, IObjectKind> _kinds = new(System.StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes the catalog with the built-in core kinds.
        /// </summary>
        static ObjectKindCatalog()
        {
            Register(new Documents.Document());
            Register(new Blogs.Blog());
            Register(new Issues.Issue());
        }

        /// <summary>
        /// Gets the registered kinds, ordered by <see cref="IObjectKind.Order"/> and
        /// then by key.
        /// </summary>
        public static IEnumerable<IObjectKind> Kinds
        {
            get
            {
                lock (_sync)
                {
                    return [.. _kinds.Values
                        .OrderBy(x => x.Order)
                        .ThenBy(x => x.Key, System.StringComparer.OrdinalIgnoreCase)];
                }
            }
        }

        /// <summary>
        /// Registers the supplied kind descriptor. The key is normalized via
        /// <see cref="Model.Entities.ObjectKind.Normalize"/>; registering a key that is
        /// already present replaces the existing descriptor, so an add-on may override
        /// the presentation of a built-in kind.
        /// </summary>
        /// <param name="kind">The kind descriptor to register. Must not be null.</param>
        public static void Register(IObjectKind kind)
        {
            System.ArgumentNullException.ThrowIfNull(kind);

            var key = Model.Entities.ObjectKind.Normalize(kind.Key);

            lock (_sync)
            {
                _kinds[key] = kind;
            }
        }

        /// <summary>
        /// Resolves a kind key to its registered descriptor. The key is normalized
        /// before the lookup, so null or whitespace resolves to the default kind.
        /// </summary>
        /// <param name="key">The kind key to resolve. May be null.</param>
        /// <returns>
        /// The registered descriptor, or <see langword="null"/> when no kind with the
        /// normalized key is registered (e.g. the key belongs to an uninstalled add-on).
        /// </returns>
        public static IObjectKind GetKind(string key)
        {
            var normalized = Model.Entities.ObjectKind.Normalize(key);

            lock (_sync)
            {
                return _kinds.TryGetValue(normalized, out var kind) ? kind : null;
            }
        }
    }
}
