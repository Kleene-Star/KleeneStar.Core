using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebSettingPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebSettingPage
{
    /// <summary>
    /// Represents the identity settings category used for configuring authentication and user identity options within
    /// the application.
    /// </summary>
    [WebIcon<IconUsers>]
    [Name("kleenestar.core:setting.category.usermanagement.name")]
    [Description("kleenestar.core:setting.category.usermanagement.description")]
    [SettingSection(SettingSection.Secondary)]
    public sealed class SettingCategoryIdentity : ISettingCategory
    {
    }
}
