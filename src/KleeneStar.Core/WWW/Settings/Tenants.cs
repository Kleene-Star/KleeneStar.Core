using KleeneStar.Core.WebManager;
using System.Linq;
using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSettingPage;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebSettingPage;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;
using KleeneStar.Model.Entities;

namespace KleeneStar.Core.WWW.Settings
{
    /// <summary>
    /// Represents the tenant management settings page, providing an overview of workspace-tenant assignments.
    /// </summary>
    [Title("kleenestar.core:setting.tenants.title")]
    [WebIcon<IconList>]
    [SettingGroup<SettingGroupSystemGeneral>()]
    [SettingSection(SettingSection.Secondary)]
    [Scope<IScopeAdmin>]
    public sealed class Tenants : ISettingPage<VisualTreeWebAppSetting>, IScopeAdmin
    {
        private readonly IWorkspaceManager _workspaceManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="workspaceManager">
        /// The workspace manager used to retrieve workspace and tenant information. Cannot be null.
        /// </param>
        public Tenants(IWorkspaceManager workspaceManager)
        {
            _workspaceManager = workspaceManager;
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebAppSetting visualTree)
        {
            // section header
            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = I18N.Translate
                (
                    renderContext,
                    "kleenestar.core:setting.tenants.header"
                ),
                TextColor = new PropertyColorText(TypeColorText.Info),
                Margin = new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = I18N.Translate
                (
                    renderContext,
                    "kleenestar.core:setting.tenants.description"
                ),
                Margin = new PropertySpacingMargin(PropertySpacing.Space.Two)
            });

            // build the tenant-to-workspace assignment table
            var table = new ControlTable()
            {
                Striped = TypeStripedTable.Row
            }
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:setting.tenants.column.workspace"))
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:setting.tenants.column.tenant"))
                .AddColumn(I18N.Translate(renderContext, "kleenestar.core:setting.tenants.column.count"));

            // enumerate workspaces and their tenant assignments
            var query = new Query<Workspace>();
            var workspaces = _workspaceManager.GetWorkspaces(query);

            foreach (var workspace in workspaces)
            {
                var tenants = workspace.Tenants;
                var tenantNames = tenants?.Select(t => t.Name).Where(n => !string.IsNullOrEmpty(n));
                var tenantDisplay = tenantNames != null && tenantNames.Any()
                    ? string.Join(", ", tenantNames)
                    : I18N.Translate(renderContext, "kleenestar.core:workspace.property.none");
                var tenantCount = tenants?.Count() ?? 0;

                table.AddRow
                (
                    new ControlTableCell() { Text = workspace.Name },
                    new ControlTableCell() { Text = tenantDisplay },
                    new ControlTableCell() { Text = tenantCount.ToString() }
                );
            }

            visualTree.Content.MainPanel.AddPrimary(table);
        }
    }
}
