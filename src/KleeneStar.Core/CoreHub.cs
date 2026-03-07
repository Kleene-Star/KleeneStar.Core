using KleeneStar.Core.WebManager;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WebExpress.WebCore;
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
        private static ObjectManager _objectManager;

        /// <summary>
        /// Returns the shared instance of the component hub used for managing and coordinating application components.
        /// </summary>
        public static IComponentHub ComponentHub { get; internal set; }

        /// <summary>
        /// Returns the current application context, which provides access to application-wide services and configurations.
        /// </summary>
        public static IApplicationContext ApplicationContet { get; internal set; }

        /// <summary>
        /// Returns the current HTTP server context for the application.
        /// </summary>
        public static IHttpServerContext HttpServerContext { get; internal set; }

        /// <summary>
        /// Returns the workspace manager responsible for managing workspaces within the application.
        /// </summary>
        public static IWorkspaceManager WorkspaceManager => _workspaceManager ??= ComponentHub.GetComponentManager<WorkspaceManager>();

        /// <summary>
        /// Returns the class manager responsible for managing classes within the workspace.
        /// </summary>
        public static IClassManager ClassManager => _classManager ??= ComponentHub.GetComponentManager<ClassManager>();

        /// <summary>
        /// Returns the object manager responsible for managing objects within the workspace.
        /// </summary>
        public static IObjectManager ObjectManager => _objectManager ??= ComponentHub.GetComponentManager<ObjectManager>();

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

        /// <summary>
        /// Creates and displays a notification with the specified header and message.
        /// </summary>
        /// <param name="header">
        /// The title or heading text to display in the notification. Cannot be null.
        /// </param>
        /// <param name="message">
        /// The main content or body text of the notification. Cannot be null.
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
            return ComponentHub.GetComponentManager<NotificationManager>()?.AddNotification
            (
                applicationContext: WebEx.ComponentHub.ApplicationManager.GetApplication<KleeneStarApplication>(),
                icon: ApplicationContet.Icon?.ToUri()?.ToString(),
                heading: header,
                message: message,
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
        /// creates the icons directory if it does not already exist.
        /// </remarks>
        /// <param name="id">
        /// The unique identifier used to select the icon color and determine the icon file name.
        /// </param>
        /// <returns>
        /// An IIcon instance representing the generated SVG icon, accessible via a relative URI endpoint.
        /// </returns>
        public static ImageIcon GenerateIcon(Guid id)
        {
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

            // load the embedded kleenestar.svg resource from assembly
            var assembly = typeof(WorkspaceManager).Assembly;
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(x => x.EndsWith("KleeneStar.Core.Assets.img\\kleenestar.svg", StringComparison.OrdinalIgnoreCase))
                ?? throw new FileNotFoundException("Embedded kleenestar.svg resource not found.");

            // define target icon directory and icon filename
            var iconDirectory = Path.Combine(AppContext.BaseDirectory, HttpServerContext?.DataPath, "icons");
            var iconFileName = $"{id}.svg";

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
            var outputPath = Path.Combine(iconDirectory, iconFileName);
            File.WriteAllText(outputPath, newContent);

            return new ImageIcon(ApplicationContet.Route.Concat($"/assets/icons/{iconFileName}").ToUri());
        }
    }
}
