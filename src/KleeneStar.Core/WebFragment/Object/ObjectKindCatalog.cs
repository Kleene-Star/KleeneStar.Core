using System.Collections.Generic;
using System.Linq;
using WebExpress.WebCore.WebUri;

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
            Register(new Assets.Asset());
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

        /// <summary>
        /// Resolves the detail (reading) view route of the supplied object, addressed by
        /// its kind. This is the single dispatch point every object link uses now that
        /// the detail view is split per kind (<c>/issue</c>, <c>/document</c>,
        /// <c>/blog</c>, …).
        /// </summary>
        /// <param name="object">The object to link to. May be null.</param>
        /// <returns>The bound detail route, or <see langword="null"/> when the object is
        /// null.</returns>
        public static IUri ResolveDetailUri(Model.Entities.Object @object)
        {
            return @object is null ? null : ResolveDetailUri(@object.Kind, @object.Key);
        }

        /// <summary>
        /// Resolves the detail (reading) view route for the supplied kind key and object
        /// key. An unknown kind (e.g. the key of an uninstalled add-on) falls back to the
        /// issue detail view so the link still resolves to something meaningful.
        /// </summary>
        /// <param name="kind">The object's kind key. May be null.</param>
        /// <param name="objectKey">The object's key. May be null.</param>
        /// <returns>The bound detail route.</returns>
        public static IUri ResolveDetailUri(string kind, string objectKey)
        {
            var descriptor = GetKind(kind) ?? GetKind(Model.Entities.ObjectKind.Issue);

            return descriptor?.DetailUri(objectKey);
        }

        /// <summary>
        /// Resolves the detail route of the supplied object with its trailing object-key
        /// path segment "frozen" into a constant.
        /// </summary>
        /// <remarks>
        /// Some controls (notably the sidebar link) re-bind the URI they render against
        /// the current request. On a detail page the request already carries an object
        /// key, and <c>BindParameters</c> overwrites <em>any</em> matching variable
        /// segment — so a link that targets a <em>different</em> object of the same kind
        /// would be rewritten to point at the current object. Rebuilding the object-key
        /// segment as a constant makes the URI immune to that re-bind while leaving the
        /// (already constant) route and kind segments untouched. Use this for object links
        /// rendered inside such a control on a detail page (e.g. the document tree and blog
        /// timeline).
        /// </remarks>
        /// <param name="object">The object to link to. May be null.</param>
        /// <returns>The frozen detail route, or <see langword="null"/> when the object is null.</returns>
        public static IUri ResolveDetailUriFrozen(Model.Entities.Object @object)
        {
            var uri = ResolveDetailUri(@object);

            if (uri is null)
            {
                return null;
            }

            var segments = uri.PathSegments.ToList();

            // the object-key is the trailing (and only variable) segment of a detail route;
            // keep the leading constants and re-append it as a constant
            if (segments.Count < 2)
            {
                return uri;
            }

            return uri.Take(segments.Count - 1).Concat(segments[^1].ToString());
        }

        /// <summary>
        /// Resolves the dedicated edit view route of the supplied object, addressed by
        /// its kind. Returns <see langword="null"/> for kinds that edit inline / via a
        /// modal (such as the issue kind).
        /// </summary>
        /// <param name="object">The object to edit. May be null.</param>
        /// <returns>The bound edit route, or <see langword="null"/>.</returns>
        public static IUri ResolveEditUri(Model.Entities.Object @object)
        {
            return @object is null ? null : ResolveEditUri(@object.Kind, @object.Key);
        }

        /// <summary>
        /// Resolves the dedicated edit view route for the supplied kind key and object
        /// key. Returns <see langword="null"/> when the kind has no dedicated edit route.
        /// </summary>
        /// <param name="kind">The object's kind key. May be null.</param>
        /// <param name="objectKey">The object's key. May be null.</param>
        /// <returns>The bound edit route, or <see langword="null"/>.</returns>
        public static IUri ResolveEditUri(string kind, string objectKey)
        {
            var descriptor = GetKind(kind) ?? GetKind(Model.Entities.ObjectKind.Issue);

            return descriptor?.EditUri(objectKey);
        }
    }
}
