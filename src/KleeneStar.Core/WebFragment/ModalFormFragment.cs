using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a modal form for all purpose within the application.
    /// Section <see cref="SectionBodySecondary"/>: <see cref="WebExpress.WebApp.WebSettingPage.VisualTreeWebAppSetting"/>
    /// (the visual tree used by setting pages like Tenants/Groups/Identities)
    /// does not render the epilogue sections, so a fragment in
    /// <c>SectionEpiloguePrimary</c> would never reach the DOM there and the
    /// modal target id <c>modal-form</c> would be missing for the Add/Edit/
    /// Clone/Delete buttons on those pages.
    /// </summary>
    [Section<SectionBodySecondary>]
    [Scope<IScopeGeneral>]
    [Scope<IScopeAdmin>]
    [Cache]
    public sealed class ModalFormFragment : FragmentControlModalRemoteForm
    {
        /// <summary>
        /// Initializes a new instance of the class using the 
        /// specified fragment context.
        /// </summary>
        public ModalFormFragment(IFragmentContext fragmentContext)
            : base(fragmentContext, "modal-form")
        {
        }
    }
}
