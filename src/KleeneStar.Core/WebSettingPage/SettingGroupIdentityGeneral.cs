using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebSettingPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebSettingPage
{
    /// <summary>
    /// Represents the general identity settings group within the application's configuration system.
    /// </summary>
    [WebIcon<IconUsers>]
    [Name("kleenestar.core:setting.group.usermanagement.name")]
    [Description("kleenestar.core:setting.group.usermanagement.description")]
    [SettingSection(SettingSection.Secondary)]
    [SettingCategory<SettingCategoryIdentity>]
    public sealed class SettingGroupIdentityGeneral : ISettingGroup
    {
    }
}
