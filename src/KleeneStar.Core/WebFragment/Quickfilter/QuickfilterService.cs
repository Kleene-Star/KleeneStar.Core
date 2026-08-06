using KleeneStar.Core.WebParameter;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Quickfilter
{
    /// <summary>
    /// Resolves which quickfilter endpoint a filter dialog reads and writes through.
    /// </summary>
    /// <remarks>
    /// The dialogs serve every bar, so the endpoint follows from the view the chip named rather
    /// than from an address handed in — a dialog cannot be pointed at something else that way.
    /// A further bar is added by naming its view here.
    ///
    /// A bar of a view that exists once per workspace needs that workspace as well, and the dialog
    /// cannot take it from its own route: the dialog is a page of its own and its route names no
    /// workspace. The chip therefore carries it along, and it is bound into the endpoint address
    /// here — without that the address keeps its <c>${workspacekey}</c> placeholder and the dialog
    /// asks for a route that does not exist.
    /// </remarks>
    internal static class QuickfilterService
    {
        /// <summary>
        /// Returns the address of the quickfilter endpoint the named bar is served by.
        /// </summary>
        /// <param name="renderContext">The context the dialog is rendered in.</param>
        /// <returns>
        /// The address, or null when the view is not one that has a bar with user-defined filters,
        /// or when a per-workspace bar was named without its workspace.
        /// </returns>
        public static string Resolve(IRenderControlContext renderContext)
        {
            var view = renderContext?.Request?.GetParameter("view")?.Value;
            var context = renderContext?.Request?.GetParameter("context")?.Value;

            switch (view)
            {
                case global::KleeneStar.Core.WWW.Api._1_.Tenants.Quickfilter.ViewKey:
                    return CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Tenants.Quickfilter>()?.ToString();

                case global::KleeneStar.Core.WWW.Api._1_.Issues._workspacekey_.Quickfilter.ViewKey:
                    // answering with an unbound address would send the dialog to a route that does
                    // not exist, which reads as a missing endpoint rather than a missing workspace
                    if (string.IsNullOrWhiteSpace(context))
                    {
                        return null;
                    }

                    return CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Issues._workspacekey_.Quickfilter>()?
                        .BindParameters(new WorkspaceKeyParameter(context))?
                        .ToString();

                default:
                    return null;
            }
        }
    }
}
