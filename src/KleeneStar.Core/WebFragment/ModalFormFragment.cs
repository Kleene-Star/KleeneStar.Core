using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;

namespace KleeneStar.Core.WebFragment
{
    /// <summary>
    /// Represents a modal form for all porpose within the application.
    /// </summary>
    [Section<SectionContentSecondary>]
    [Scope<IScopeGeneral>]
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
