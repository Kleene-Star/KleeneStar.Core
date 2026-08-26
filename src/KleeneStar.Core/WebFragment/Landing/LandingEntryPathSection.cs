using KleeneStar.Core.WebManager;
using System;
using System.Linq;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebCore.WebUri;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Landing
{
    /// <summary>
    /// The entry-path area: the four ways into the work, each named, explained in one
    /// sentence, sized, and linked to the view that shows it.
    /// </summary>
    /// <remarks>
    /// This is the section that turns the landing page from a status display into a starting
    /// point. Three of the four paths are personal - what is mine, what was shared with me,
    /// what I am watching - and one is the organization's: the workspaces, where issues that
    /// belong to nobody in particular are found. The figure on a card is the size of the slice
    /// behind it, counted through the same definition the target page lists
    /// (<see cref="LandingScope"/>), so card and page cannot disagree.
    /// </remarks>
    internal static class LandingEntryPathSection
    {
        /// <summary>
        /// Builds the section.
        /// </summary>
        /// <param name="objectManager">The object manager used to size the personal slices.</param>
        /// <param name="workspaceManager">The workspace manager. Reserved for the organization slice.</param>
        /// <param name="shareManager">The share manager naming the shared objects.</param>
        /// <param name="watcherManager">The watcher manager naming the watched objects.</param>
        /// <param name="renderContext">The render context.</param>
        /// <returns>The section control.</returns>
        public static IControl Build
        (
            IObjectManager objectManager,
            IWorkspaceManager workspaceManager,
            IShareManager shareManager,
            IWatcherManager watcherManager,
            IRenderControlContext renderContext
        )
        {
            var identityId = CoreHub.SessionManager.GetCurrentIdentityId(renderContext?.Request);

            var section = new ControlSection("landing-paths")
            {
                Header = _ => "kleenestar.core:landing.paths.heading",
                HeaderIcon = _ => new IconSignsPost(),
                Note = _ => "kleenestar.core:landing.paths.hint",
                Layout = _ => TypeLayoutSection.Rule
            };

            // a grid rather than a row of tiles: the four paths are one set, they divide the
            // width between them, and the rule between the fields says they belong together
            var tiles = new ControlGroup("landing-paths-tiles")
            {
                Columns = _ => 2,
                Spacing = _ => TypeSpacingGroup.Wide
            };

            tiles.Add
            (
                BuildCard
                (
                    renderContext,
                    "mine",
                    "kleenestar.core:landing.paths.mine.label",
                    "kleenestar.core:landing.paths.mine.description",
                    new IconInbox(),
                    CountMine(objectManager, identityId),
                    CoreHub.GetUri<global::KleeneStar.Core.WWW.Mine.Index>()
                ),
                BuildCard
                (
                    renderContext,
                    "org",
                    "kleenestar.core:landing.paths.org.label",
                    "kleenestar.core:landing.paths.org.description",
                    new IconGlobe(),
                    CountWorkspaceIssues(objectManager),
                    CoreHub.GetUri<global::KleeneStar.Core.WWW.Workspaces.Index>()
                ),
                BuildCard
                (
                    renderContext,
                    "shared",
                    "kleenestar.core:landing.paths.shared.label",
                    "kleenestar.core:landing.paths.shared.description",
                    new IconShareNodes(),
                    CountShared(objectManager, shareManager, identityId),
                    CoreHub.GetUri<global::KleeneStar.Core.WWW.Shared.Index>()
                ),
                BuildCard
                (
                    renderContext,
                    "watched",
                    "kleenestar.core:landing.paths.watched.label",
                    "kleenestar.core:landing.paths.watched.description",
                    new IconEye(),
                    CountWatched(objectManager, watcherManager, identityId),
                    CoreHub.GetUri<global::KleeneStar.Core.WWW.Watched.Index>()
                )
            );

            section.Add(tiles);

            return section;
        }

        /// <summary>
        /// Builds the card of a single entry path: its icon, its name, the size of its slice as
        /// the chip, the sentence that explains it, and the link that opens the view behind it.
        /// </summary>
        /// <param name="renderContext">The render context used for translating.</param>
        /// <param name="key">The short id suffix of the path.</param>
        /// <param name="label">The resource key of the name.</param>
        /// <param name="description">The resource key of the explanatory sentence.</param>
        /// <param name="icon">The icon of the card.</param>
        /// <param name="count">The size of the slice behind the card.</param>
        /// <param name="uri">The route of the view behind the card.</param>
        /// <returns>The card.</returns>
        private static IControl BuildCard
        (
            IRenderControlContext renderContext,
            string key,
            string label,
            string description,
            IIcon icon,
            int count,
            IUri uri
        )
        {
            var chip = LandingHtml.Number(count, renderContext);
            var panel = new ControlPanel("landing-path-" + key);

            panel.Add(new ControlLink("landing-path-open-" + key)
            {
                Text = _ => label,
                Icon = _ => icon,
                Uri = _ => uri
            });

            panel.Add(new ControlBadge("landing-path-count-" + key)
            {
                Value = _ => chip,
                BackgroundColor = _ => new PropertyColorBackgroundBadge(TypeColorBackgroundBadge.Secondary)
            });

            panel.Add(new ControlText("landing-path-description-" + key)
            {
                Text = _ => description,
                TextColor = _ => new PropertyColorText(TypeColorText.Secondary),
                Format = _ => TypeFormatText.Paragraph
            });

            return panel;
        }

        /// <summary>
        /// Counts the issues that belong to the supplied identity.
        /// </summary>
        /// <param name="objectManager">The object manager.</param>
        /// <param name="identityId">The calling identity.</param>
        /// <returns>The size of the slice, or <c>0</c> without an identity.</returns>
        private static int CountMine(IObjectManager objectManager, Guid identityId)
        {
            return identityId == Guid.Empty
                ? 0
                : objectManager.CountObjects(LandingScope.BuildMineQuery(identityId));
        }

        /// <summary>
        /// Counts the objects shared with the supplied identity.
        /// </summary>
        /// <param name="objectManager">The object manager.</param>
        /// <param name="shareManager">The share manager.</param>
        /// <param name="identityId">The calling identity.</param>
        /// <returns>The size of the slice, or <c>0</c> without an identity.</returns>
        private static int CountShared(IObjectManager objectManager, IShareManager shareManager, Guid identityId)
        {
            if (identityId == Guid.Empty)
            {
                return 0;
            }

            var ids = LandingScope.GetSharedIds(shareManager, identityId);

            return ids.Length == 0 ? 0 : objectManager.CountObjects(LandingScope.BuildIdQuery(ids));
        }

        /// <summary>
        /// Counts the objects the supplied identity is watching.
        /// </summary>
        /// <param name="objectManager">The object manager.</param>
        /// <param name="watcherManager">The watcher manager.</param>
        /// <param name="identityId">The calling identity.</param>
        /// <returns>The size of the slice, or <c>0</c> without an identity.</returns>
        private static int CountWatched(IObjectManager objectManager, IWatcherManager watcherManager, Guid identityId)
        {
            if (identityId == Guid.Empty)
            {
                return 0;
            }

            var ids = LandingScope.GetWatchedIds(watcherManager, identityId);

            return ids.Length == 0 ? 0 : objectManager.CountObjects(LandingScope.BuildIdQuery(ids));
        }

        /// <summary>
        /// Counts the open issues of the organization - what the workspaces path leads into.
        /// </summary>
        /// <remarks>
        /// The card is about the work, not about the containers holding it: a reader deciding
        /// whether to go there wants to know how much is waiting, not how many workspaces
        /// exist.
        /// </remarks>
        /// <param name="objectManager">The object manager.</param>
        /// <returns>The number of active issues.</returns>
        private static int CountWorkspaceIssues(IObjectManager objectManager)
        {
            var query = new Query<Model.Entities.Object>()
                .WhereEquals(x => x.Kind, Model.Entities.ObjectKind.Issue)
                .Where(x => x.State == Model.Entities.WorkspaceState.Active);

            return objectManager.CountObjects(query);
        }
    }
}
