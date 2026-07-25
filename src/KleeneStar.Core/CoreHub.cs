using KleeneStar.Core.WebManager;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WebExpress.WebCore;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebApplication;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebEndpoint;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebNotification;

namespace KleeneStar.Core
{
    /// <summary>
    /// Provides utility methods for working with the KleeneStar.
    /// </summary>
    public static class CoreHub
    {
        private static WorkspaceManager _workspaceManager;
        private static ClassManager _classManager;
        private static FieldManager _fieldManager;
        private static FormManager _formManager;
        private static PriorityManager _priorityManager;
        private static WorkflowManager _workflowManager;
        private static StatusManager _statusManager;
        private static ObjectManager _objectManager;
        private static DashboardManager _dashboardManager;
        private static KanbanBoardManager _kanbanBoardManager;
        private static KindDashboardManager _kindDashboardManager;
        private static TenantManager _tenantManager;
        private static IdentityManager _identityManager;
        private static GroupManager _groupManager;
        private static TemplateManager _templateManager;
        private static ObjectViewManager _objectViewManager;
        private static SlaManager _slaManager;
        private static CalendarManager _calendarManager;
        private static CommentManager _commentManager;
        private static AttachmentManager _attachmentManager;
        private static WatcherManager _watcherManager;
        private static ShareManager _shareManager;
        private static ObjectTagManager _objectTagManager;
        private static ValueManager _valueManager;
        private static ObjectLinkManager _objectLinkManager;
        private static SessionManager _sessionManager;
        private static SavedSearchManager _savedSearchManager;
        private static SprintManager _sprintManager;

        /// <summary>
        /// Gets the shared instance of the component hub used for managing and coordinating application components.
        /// </summary>
        public static IComponentHub ComponentHub { get; internal set; }

        /// <summary>
        /// Gets the current application context, which provides access to application-wide services and configurations.
        /// </summary>
        public static IApplicationContext ApplicationContext { get; internal set; }

        /// <summary>
        /// Gets the current HTTP server context for the application.
        /// </summary>
        public static IHttpServerContext HttpServerContext { get; internal set; }

        /// <summary>
        /// Gets the workspace manager responsible for managing workspaces within the application.
        /// </summary>
        public static IWorkspaceManager WorkspaceManager => _workspaceManager ??= ComponentHub.GetComponentManager<WorkspaceManager>();

        /// <summary>
        /// Gets the class manager responsible for managing classes within the workspace.
        /// </summary>
        public static IClassManager ClassManager => _classManager ??= ComponentHub.GetComponentManager<ClassManager>();

        /// <summary>
        /// Gets the field manager responsible for managing fields within the class.
        /// </summary>
        public static IFieldManager FieldManager => _fieldManager ??= ComponentHub.GetComponentManager<FieldManager>();

        /// <summary>
        /// Gets the form manager responsible for managing forms within the class.
        /// </summary>
        public static IFormManager FormManager => _formManager ??= ComponentHub.GetComponentManager<FormManager>();

        /// <summary>
        /// Gets the priority manager responsible for managing priorities within the class.
        /// </summary>
        public static IPriorityManager PriorityManager => _priorityManager ??= ComponentHub.GetComponentManager<PriorityManager>();

        /// <summary>
        /// Gets the workflow manager responsible for managing workflows within the class.
        /// </summary>
        public static IWorkflowManager WorkflowManager => _workflowManager ??= ComponentHub.GetComponentManager<WorkflowManager>();

        /// <summary>
        /// Gets the workflow state manager responsible for managing workflow states within the class.
        /// </summary>
        public static IStatusManager StatusManager => _statusManager ??= ComponentHub.GetComponentManager<StatusManager>();

        /// <summary>
        /// Gets the object manager responsible for managing objects within the workspace.
        /// </summary>
        public static IObjectManager ObjectManager => _objectManager ??= ComponentHub.GetComponentManager<ObjectManager>();

        /// <summary>
        /// Gets the dashboard manager responsible for managing dashboards within the application.
        /// </summary>
        public static IDashboardManager DashboardManager => _dashboardManager ??= ComponentHub.GetComponentManager<DashboardManager>();

        /// <summary>
        /// Gets the Kanban board manager responsible for the persisted board layout (columns,
        /// swimlanes, board filter) of the workspace object Kanban boards.
        /// </summary>
        public static IKanbanBoardManager KanbanBoardManager => _kanbanBoardManager ??= ComponentHub.GetComponentManager<KanbanBoardManager>();

        /// <summary>
        /// Gets the object-kind dashboard manager responsible for the persisted KPI board
        /// layout (columns, widgets) of the workspace object Dashboard tabs.
        /// </summary>
        public static IKindDashboardManager KindDashboardManager => _kindDashboardManager ??= ComponentHub.GetComponentManager<KindDashboardManager>();

        /// <summary>
        /// Gets the tenant manager used to manage tenant-related operations within the application.
        /// </summary>
        public static ITenantManager TenantManager => _tenantManager ??= ComponentHub.GetComponentManager<TenantManager>();

        /// <summary>
        /// Gets the identity manager used to manage identity-related operations within the application.
        /// </summary>
        public static IIdentityManager IdentityManager => _identityManager ??= ComponentHub.GetComponentManager<IdentityManager>();

        /// <summary>
        /// Gets the group manager used to manage group-related operations within the application.
        /// </summary>
        public static IGroupManager GroupManager => _groupManager ??= ComponentHub.GetComponentManager<GroupManager>();

        /// <summary>
        /// Gets the template manager responsible for managing templates within the workspace.
        /// </summary>
        public static ITemplateManager TemplateManager => _templateManager ??= ComponentHub.GetComponentManager<TemplateManager>();

        /// <summary>
        /// Gets the object view manager responsible for managing the persisted tabs that
        /// wrap the objects index of a workspace.
        /// </summary>
        public static IObjectViewManager ObjectViewManager => _objectViewManager ??= ComponentHub.GetComponentManager<ObjectViewManager>();

        /// <summary>
        /// Gets the SLA manager responsible for managing service-level-agreement policies
        /// attached to classes.
        /// </summary>
        public static ISlaManager SlaManager => _slaManager ??= ComponentHub.GetComponentManager<SlaManager>();

        /// <summary>
        /// Gets the calendar manager responsible for managing working-hours calendars
        /// attached to classes.
        /// </summary>
        public static ICalendarManager CalendarManager => _calendarManager ??= ComponentHub.GetComponentManager<CalendarManager>();

        /// <summary>
        /// Gets the comment manager responsible for managing discussion threads attached
        /// to objects.
        /// </summary>
        public static ICommentManager CommentManager => _commentManager ??= ComponentHub.GetComponentManager<CommentManager>();

        /// <summary>
        /// Gets the attachment manager responsible for the files attached to objects.
        /// </summary>
        public static IAttachmentManager AttachmentManager => _attachmentManager ??= ComponentHub.GetComponentManager<AttachmentManager>();

        /// <summary>
        /// Gets the watcher manager responsible for the per-identity watch relationships
        /// on objects.
        /// </summary>
        public static IWatcherManager WatcherManager => _watcherManager ??= ComponentHub.GetComponentManager<WatcherManager>();

        /// <summary>
        /// Gets the share manager responsible for the per-identity share relationships
        /// on objects (e.g. portal issues shared with additional tenant members).
        /// </summary>
        public static IShareManager ShareManager => _shareManager ??= ComponentHub.GetComponentManager<ShareManager>();

        /// <summary>
        /// Gets the object-tag manager responsible for the tags (labels) attached to objects.
        /// </summary>
        public static IObjectTagManager ObjectTagManager => _objectTagManager ??= ComponentHub.GetComponentManager<ObjectTagManager>();

        /// <summary>
        /// Gets the value manager responsible for the per-object per-field value rows
        /// that back the typed inputs on the object detail and edit views.
        /// </summary>
        public static IValueManager ValueManager => _valueManager ??= ComponentHub.GetComponentManager<ValueManager>();

        /// <summary>
        /// Gets the object-link manager responsible for the typed directional links
        /// between objects (e.g. blocked-by, duplicates, relates-to).
        /// </summary>
        public static IObjectLinkManager ObjectLinkManager => _objectLinkManager ??= ComponentHub.GetComponentManager<ObjectLinkManager>();

        /// <summary>
        /// Gets the session manager responsible for per-identity session/preference
        /// entries (e.g. persisted REST API table column layouts).
        /// </summary>
        public static ISessionManager SessionManager => _sessionManager ??= ComponentHub.GetComponentManager<SessionManager>();

        /// <summary>
        /// Gets the saved-search manager responsible for the per-identity saved searches
        /// that back the global search dropdown and the search-page sidebar.
        /// </summary>
        public static ISavedSearchManager SavedSearchManager => _savedSearchManager ??= ComponentHub.GetComponentManager<SavedSearchManager>();

        /// <summary>
        /// Gets the sprint manager responsible for the Scrum iterations of the
        /// workspaces and the sprint assignment of their objects.
        /// </summary>
        public static ISprintManager SprintManager => _sprintManager ??= ComponentHub.GetComponentManager<SprintManager>();

        /// <summary>
        /// Constructs a URI for the specified endpoint type using the provided parameters.
        /// </summary>
        /// <typeparam name="TEndpoint">
        /// The type of the endpoint for which the URI is being constructed.
        /// </typeparam>
        /// <param name="parameters">
        /// An array of parameters used to customize the URI construction. Can be empty.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IUri"/> representing the constructed URI for the specified endpoint.
        /// </returns>
        public static IUri GetUri<TEndpoint>(params Parameter[] parameters)
            where TEndpoint : IEndpoint
        {
            return ComponentHub.SitemapManager.GetUri<TEndpoint>(ApplicationContext, parameters);
        }

        /// <summary>
        /// Creates and displays a notification with the specified header and message.
        /// </summary>
        /// <remarks>
        /// Both <paramref name="header"/> and <paramref name="message"/> are treated as
        /// internationalization keys (e.g. <c>kleenestar.core:notification.title.created</c>)
        /// and resolved against the application's default culture via
        /// <see cref="I18N.Translate(string)"/>. A string that is not a known key is rendered
        /// verbatim, so plain text continues to work as a fallback. The global notification is
        /// not request-scoped, hence the default culture is used rather than a per-user one.
        /// </remarks>
        /// <param name="header">
        /// The i18n key of the title/heading to display in the notification. Cannot be null.
        /// </param>
        /// <param name="message">
        /// The i18n key of the main content/body text of the notification. Cannot be null.
        /// </param>
        /// <param name="durability">
        /// The duration, in milliseconds, that the notification remains visible. Specify -1
        /// to use the default duration.
        /// </param>
        /// <returns>
        /// An object representing the created notification.
        /// </returns>
        public static INotification AddNotification(string header, string message, int durability = -1)
        {
            // best-effort: callers (manager Add/Update/Remove) rely on this returning
            // null silently when the host is not fully initialized (e.g. in unit tests
            // where CoreHub is wired without a real component hub), rather than NREing.
            var notificationManager = ComponentHub?.GetComponentManager<NotificationManager>();
            if (notificationManager is null)
            {
                return null;
            }

            var application = WebEx.ComponentHub?.ApplicationManager?.GetApplication<KleeneStarApplication>();
            if (application is null)
            {
                return null;
            }

            return notificationManager.AddNotification
            (
                applicationContext: application,
                icon: ApplicationContext?.Icon?.ToUri()?.ToString(),
                heading: I18N.Translate(header),
                message: I18N.Translate(message),
                durability: durability
            );
        }

        /// <summary>
        /// Generates a unique SVG icon for the specified identifier and saves it to the icons directory.
        /// </summary>
        /// <remarks>
        /// The icon color is selected from a palette of 32 distinct colors based on the hash
        /// code of the provided identifier. The generated icon is saved as an SVG file in the
        /// application's icons directory and can be accessed via a URI endpoint. This method
        /// creates the icons directory if it does not already exist. Because the file content
        /// is fully determined by the identifier, an already generated icon is reused as-is
        /// instead of being regenerated and rewritten.
        /// </remarks>
        /// <param name="id">
        /// The unique identifier used to select the icon color and determine the icon file name.
        /// </param>
        /// <returns>
        /// An IIcon instance representing the generated SVG icon, accessible via a relative URI endpoint.
        /// </returns>
        public static ImageIcon GenerateIcon(Guid id)
        {
            // define target icon directory, file name, and the public URI of the icon
            var iconDirectory = Path.Combine(AppContext.BaseDirectory, HttpServerContext?.DataPath, "icons");
            var iconFileName = $"{id}.svg";
            var outputPath = Path.Combine(iconDirectory, iconFileName);
            var icon = new ImageIcon(ApplicationContext.Route.Concat($"/assets/icons/{iconFileName}").ToUri());

            // the icon is a deterministic function of the id: the same id always maps to
            // the same color and therefore the same file content. If it has already been
            // generated, reuse the file on disk instead of re-reading the embedded
            // template, re-running the regex, and rewriting identical bytes — this path
            // runs on every entity create/edit, so the short-circuit avoids redundant I/O.
            if (File.Exists(outputPath))
            {
                return icon;
            }

            // color palette: 32 distinct, contrast-rich colors
            var colors = new[]
            {
                "#ca1554", "#25509f", "#008237", "#b76f13", "#404b91", "#368b22", "#953599", "#ed381e",
                "#167ca0", "#d4424f", "#513e21", "#0e6a73", "#8b2443", "#20723d", "#6c2122", "#3b7d8d",
                "#1b6d44", "#903525", "#221f53", "#41775c", "#bd6b82", "#224f44", "#6a3ba1", "#387251",
                "#c26f1b", "#38464a", "#752b5c", "#09897c", "#998f35", "#da4040", "#2a537d", "#146459"
            };

            // calculate color index based on GUID
            var bytes = id.ToByteArray();
            int colorIndex = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                colorIndex = (colorIndex * 31 + bytes[i]) % colors.Length;
                if (colorIndex < 0) { colorIndex += colors.Length; } // safety for negatives
            }

            var colorHex = colors[colorIndex];

            // load the embedded kleenestar.svg resource from assembly. The manifest name is
            // produced from the csproj LogicalName template, whose %(RecursiveDir) token uses
            // the build host's directory separator ('\' on Windows, '/' elsewhere). Normalize
            // both separators to '.' before matching so icon generation works regardless of
            // the platform the assembly was built on.
            var assembly = typeof(WorkspaceManager).Assembly;
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(x => x.Replace('\\', '.').Replace('/', '.')
                    .EndsWith("KleeneStar.Core.Assets.img.kleenestar.svg", StringComparison.OrdinalIgnoreCase))
                ?? throw new FileNotFoundException("Embedded kleenestar.svg resource not found.");

            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException("SVG asset stream not found.");
            using var reader = new StreamReader(stream);
            var svgContent = reader.ReadToEnd();

            // replace the fill attribute in the first <rect> element with the selected color
            var newContent = Regex.Replace(
                svgContent,
                @"(<rect\b[^>]*\bfill\s*=\s*[""']?)[^""'>]+([""']?)",
                $"$1{colorHex}$2",
                RegexOptions.IgnoreCase
            );

            // create the icon directory if it does not exist
            Directory.CreateDirectory(iconDirectory);

            // write the modified SVG to the icon file
            File.WriteAllText(outputPath, newContent);

            return icon;
        }
    }
}
