using KleeneStar.Core.Workspace;
using WebExpress.WebCore.WebApplication;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebEndpoint;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;

namespace KleeneStar.Core
{
    /// <summary>
    /// Provides utility methods for working with the KleeneStar.
    /// </summary>
    public static class CoreHub
    {
        private static WorkspaceManager _workspaceManager;

        /// <summary>
        /// Returns the shared instance of the component hub used for managing and coordinating application components.
        /// </summary>
        public static IComponentHub ComponentHub { get; internal set; }

        /// <summary>
        /// Returns the current application context, which provides access to application-wide services and configurations.
        /// </summary>
        public static IApplicationContext ApplicationContet { get; internal set; }

        /// <summary>
        /// Returns the workspace manager responsible for managing workspaces within the application.
        /// </summary>
        public static IWorkspaceManager WorkspaceManager => _workspaceManager ??= ComponentHub.GetComponentManager<WorkspaceManager>();

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
            return ComponentHub.SitemapManager.GetUri<TEndpoint>(ApplicationContet, parameters);
        }
    }
}
