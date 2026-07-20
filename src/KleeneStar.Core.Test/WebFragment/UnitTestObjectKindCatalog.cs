using KleeneStar.Core.WebFragment.Object;
using KleeneStar.Model.Entities;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core.Test.WebFragment
{
    /// <summary>
    /// Provides unit tests for the <see cref="ObjectKindCatalog"/> — the extensible
    /// registry behind the object kinds (subtypes).
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestObjectKindCatalog
    {
        /// <summary>
        /// A minimal add-on style kind used to exercise the registration path. The
        /// overview URI stays null — resolving it would require a running host.
        /// </summary>
        /// <param name="key">The kind key to register under.</param>
        private sealed class TestKind(string key) : IObjectKind
        {
            /// <summary>
            /// Gets the persisted kind key.
            /// </summary>
            public string Key => key;

            /// <summary>
            /// Gets the internationalization key of the display name.
            /// </summary>
            public string Label => $"test:{key}.label";

            /// <summary>
            /// Gets the icon representing the kind (none for the test double).
            /// </summary>
            public IIcon Icon => null!;

            /// <summary>
            /// Gets the display order (after the built-in kinds).
            /// </summary>
            public int Order => 100;

            /// <summary>
            /// Gets the overview route (none for the test double).
            /// </summary>
            public IUri OverviewUri => null!;
        }

        /// <summary>
        /// Verifies that the built-in kinds are registered with their persisted keys and
        /// appear in their declared order.
        /// </summary>
        [Fact]
        public void Kinds_ContainBuiltInsInOrder()
        {
            var keys = ObjectKindCatalog.Kinds.Select(k => k.Key).ToList();

            var document = keys.IndexOf(ObjectKind.Document);
            var blog = keys.IndexOf(ObjectKind.Blog);
            var issue = keys.IndexOf(ObjectKind.Issue);

            Assert.True(document >= 0, "expected the document kind to be registered");
            Assert.True(blog >= 0, "expected the blog kind to be registered");
            Assert.True(issue >= 0, "expected the issue kind to be registered");
            Assert.True(document < blog && blog < issue, "expected the built-in order documents, blogs, issues");
        }

        /// <summary>
        /// Verifies the lookup normalization: keys resolve case-insensitively, null and
        /// whitespace fall back to the default kind, and unknown keys yield null.
        /// </summary>
        [Fact]
        public void GetKind_NormalizesAndResolves()
        {
            Assert.Equal(ObjectKind.Document, ObjectKindCatalog.GetKind(" Document ")?.Key);
            Assert.Equal(ObjectKind.Default, ObjectKindCatalog.GetKind(null)?.Key);
            Assert.Equal(ObjectKind.Default, ObjectKindCatalog.GetKind("   ")?.Key);
            Assert.Null(ObjectKindCatalog.GetKind("kind-of-an-uninstalled-addon"));
        }

        /// <summary>
        /// Verifies the add-on extension path: a registered custom kind becomes
        /// resolvable and appears in the kind listing, and re-registering the same key
        /// replaces the descriptor instead of duplicating it.
        /// </summary>
        [Fact]
        public void Register_AddsAndReplacesCustomKind()
        {
            var first = new TestKind("Meeting-Notes");
            ObjectKindCatalog.Register(first);

            // the key is normalized on registration
            var resolved = ObjectKindCatalog.GetKind("meeting-notes");
            Assert.Same(first, resolved);
            Assert.Contains(ObjectKindCatalog.Kinds, k => ReferenceEquals(k, first));

            // re-registering the (differently cased) key replaces, never duplicates
            var second = new TestKind("MEETING-NOTES");
            ObjectKindCatalog.Register(second);

            Assert.Same(second, ObjectKindCatalog.GetKind("meeting-notes"));
            Assert.Single(ObjectKindCatalog.Kinds, k => ObjectKind.Normalize(k.Key) == "meeting-notes");
        }
    }
}
