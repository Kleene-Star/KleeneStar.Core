using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.Model.Workspace
{
    /// <summary>
    /// Defines the contract for managing workspaces, including adding, retrieving, and removing workspaces, as well as
    /// handling workspace-related events.
    /// </summary>
    /// <remarks>
    /// The interface provides methods for managing workspaces and events for tracking changes 
    /// to the workspace collection. Implementations of this interface should ensure thread
    /// safety if used in a multi-threaded environment.
    /// </remarks>
    public sealed class WorkspaceManager : IWorkspaceManager
    {
        private readonly IComponentHub _componentHub;
        private readonly IHttpServerContext _httpServerContext;
        private readonly Dictionary<string, IWorkspace> _workspaceMap = [];

        /// <summary>
        /// An event that fires when an workspace is added.
        /// </summary>
        public event EventHandler<IWorkspace> WorkspaceAdded;

        /// <summary>
        /// An event that fires when an workspace is udpated.
        /// </summary>
        public event EventHandler<IWorkspace> WorkspaceUpdated;

        /// <summary>
        /// An event that fires when an workspace is removed.
        /// </summary>
        public event EventHandler<IWorkspace> WorkspaceRemoved;

        /// <summary>
        /// Returns all workspaces.
        /// </summary>
        public IEnumerable<IWorkspace> Workspaces => _workspaceMap.Values;

        /// <summary>
        /// Returns the collection of workspace keys that are reserved and cannot be used for custom workspaces.
        /// </summary>
        /// <remarks>
        /// The reserved keys typically represent system-defined workspaces and are not available
        /// for user-defined or custom workspace creation.
        /// </remarks>
        public static IEnumerable<string> ReservedWorkspaceKeys =>
        [
            "default", "admin", "system", "assets", "api", "workspace",
            "workspaces", "icons", "setting"
        ];

        /// <summary>
        /// Returns the collection of category names associated with the workspace.
        /// </summary>
        public IEnumerable<string> WorkspaceCategories => Workspaces
            .Select(x => x.Categories)
            .SelectMany(x => x)
            .Where(x => x is not null)
            .Distinct();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via Reflection.")]
        private WorkspaceManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            _componentHub = componentHub;
            _httpServerContext = httpServerContext;

            var workspaceDirectory = Path.Combine(AppContext.BaseDirectory, _httpServerContext.DataPath, "workspaces");
            foreach (var workspace in LoadAllWorkspaces(workspaceDirectory))
            {
                _workspaceMap[workspace?.Key] = workspace;
            }
        }

        /// <summary>
        /// Adds a workspace to the workspace manager.
        /// </summary>
        /// <param name="workspace">The workspace to add. Cannot be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkspaceManager AddWorkspace(IWorkspace workspace)
        {
            var workspaceDirectory = Path.Combine(AppContext.BaseDirectory, _httpServerContext.DataPath, "workspaces");
            ArgumentNullException.ThrowIfNull(workspace);

            SaveWorkspace(workspace, workspaceDirectory);

            if (!_workspaceMap.ContainsKey(workspace.Key))
            {
                _workspaceMap[workspace.Key] = workspace;
            }

            WorkspaceAdded?.Invoke(this, workspace);

            return this;
        }

        /// <summary>
        /// Returns a workspace based on its id.
        /// </summary>
        /// <param name="workspaceId">The id of the workspace.</param>
        /// <returns>The workspace.</returns>
        public IWorkspace GetWorkspace(Guid workspaceId)
        {
            return Workspaces
                .Where(x => x.Id == workspaceId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the workspace associated with the specified unique key.
        /// </summary>
        /// <param name="key">
        /// The unique identifier for the workspace to retrieve. Cannot be null or empty.
        /// </param>
        /// <returns>
        /// An workspace corresponding to the specified key, or null if no matching 
        /// workspace is found.
        /// </returns>
        public IWorkspace GetWorkspaceByKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            return _workspaceMap.TryGetValue(key, out var workspace) ? workspace : null;
        }

        /// <summary>
        /// Retrieves a collection of workspaces that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="predicate"
        /// >An expression used to filter workspaces. Only workspaces for which the predicate 
        /// evaluates to true are included in the result.
        /// </param>
        /// <returns>
        /// An enumerable collection of workspaces that match the given predicate. If no workspaces 
        /// match, the collection will be empty.
        /// </returns>
        public IEnumerable<IWorkspace> GetWorkspaces(Expression<Func<IWorkspace, bool>> predicate)
        {
            return null;
        }

        /// <summary>
        /// Removes the specified workspace from the workspace manager.
        /// </summary>
        /// <remarks>This method removes the specified workspace from the manager. If the workspace does
        /// not exist in the manager, no action is taken.</remarks>
        /// <param name="workspaceId">The workspace id to be removed. Must not be null.</param>
        /// <returns>The current instance to allow for method chaining.</returns>
        public IWorkspaceManager RemoveWorkspace(Guid workspaceId)
        {
            var workspace = GetWorkspace(workspaceId);

            if (workspace is not null)
            {
                _workspaceMap.Remove(workspace?.Key);
                WorkspaceRemoved?.Invoke(this, workspace);

                var workspaceDirectory = Path.Combine(AppContext.BaseDirectory, _httpServerContext.DataPath, "workspaces");
                if (File.Exists(Path.Combine(workspaceDirectory, $"{workspace?.Key}.xml")))
                {
                    File.Delete(Path.Combine(workspaceDirectory, $"{workspace?.Key}.xml"));
                }
            }

            return this;
        }

        /// <summary>
        /// Release of unmanaged resources reserved during use.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Loads all workspace definitions from XML files in the specified directory.
        /// </summary>
        /// <param name="directoryPath">
        /// The path to the directory containing the XML files to load. The directory must exist.
        /// </param>
        /// <returns>
        /// A list of <see cref="IWorkspace"/> instances representing the loaded workspaces. The list will be empty if
        /// no valid workspaces are found.
        /// </returns>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified <paramref name="directoryPath"/> does not exist.
        /// </exception>
        public static List<IWorkspace> LoadAllWorkspaces(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return [];
            }

            return Directory.GetFiles(directoryPath, "*.xml")
                .Select(file =>
                {
                    try
                    {
                        var doc = XDocument.Load(file);
                        var root = doc.Root;
                        var iconPath = root?.Element("Icon")?.Value;
                        var iconUri = !string.IsNullOrWhiteSpace(iconPath)
                            ? KleeneStar.ApplicationContet.Route.ToUri().Concat(iconPath)
                            : null;

                        return new Workspace()
                        {
                            Id = Guid.Parse(root?.Element("Id")?.Value ?? ""),
                            Key = root?.Element("Key")?.Value ?? "",
                            Name = root?.Element("Name")?.Value ?? "",
                            Icon = iconUri is not null ? new ImageIcon(iconUri) : null,
                            State = Enum.TryParse<TypeWorkspaceState>(root?.Element("State")?.Value, out var state)
                                ? state
                                : TypeWorkspaceState.Active,
                            Description = root?.Element("Description")?.Value ?? "",
                            Categories = root?.Elements("Category").Select(e => e.Value).ToList() ?? []
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading {file}: {ex.Message}");
                        return null as IWorkspace;
                    }
                })
                .Where(ws => ws is not null)
                .ToList()!;
        }

        /// <summary>
        /// Saves a workspace definition as an XML file in the specified directory.
        /// </summary>
        /// <param name="workspace">The workspace instance to save.</param>
        /// <param name="directoryPath">
        /// The path to the directory where the XML file will be saved. The directory must exist.
        /// </param>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified <paramref name="directoryPath"/> does not exist.
        /// </exception>
        public static void SaveWorkspace(IWorkspace workspace, string directoryPath)
        {
            ArgumentNullException.ThrowIfNull(workspace?.Key);

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
            }

            var doc = new XDocument(
                new XElement("Workspace",
                    new XElement("Id", workspace.Id),
                    new XElement("Key", workspace.Key),
                    new XElement("Name", workspace.Name),
                    new XElement("Icon", (workspace.Icon as ImageIcon)?.Uri?.ToString() ?? ""),
                    new XElement("State", workspace.State.ToString()),
                    new XElement("Description", workspace.Description ?? ""),
                    workspace.Categories?.Select(c => new XElement("Category", c))
                )
            );

            var filePath = Path.Combine(directoryPath, $"{workspace.Key}.xml");
            doc.Save(filePath);
        }
    }
}
