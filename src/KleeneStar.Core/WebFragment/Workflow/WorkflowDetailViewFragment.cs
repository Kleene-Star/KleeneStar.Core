using KleeneStar.Core.WebParameter;
using WebExpress.WebApp.WebData;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Workflow
{
    /// <summary>
    /// Represents a fragment that displays detailed information for a workflow, supporting rendering in various
    /// workflow views such as create, edit, and view.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Scope<global::KleeneStar.Core.WWW.Workflow._workflowid_.Index>]
    [Cache]
    public sealed class WorkflowDetailViewFragment : FragmentControlDataWorkflow
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public WorkflowDetailViewFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            // the editor reads the workflow id from the state island and rides it along as the id
            // query parameter of both the load and the debounced autosave
            StateFactory = renderContext => DataState.Create()
                .Set("id", renderContext?.Request?.GetParameter<WorkflowIdParameter>()?.Value);

            // the editor loads the workflow definition with GET and persists it with PUT
            ServiceFactory = renderContext => DataServiceDescriptor.Data(GetUri(renderContext)?.ToString());

            // the canvas positions are persisted per workflow, so a grid pays off: what the user
            // aligns stays aligned across reloads. The cell matches the seeded layout spacing.
            Grid = _ => 20;
            GridSnap = _ => true;

            // the editor needs a definite height and defaults to 600px. Measuring against the
            // viewport rather than the parent gives the designer the room it wants without
            // depending on an unbroken chain of parent heights; the control's own min-height
            // keeps it usable on a short viewport.
            Styles = ["--wx-we-host-height: calc(100vh - 260px);"];
        }

        /// <summary>
        /// Generates a URI for the workflow editor resource based on the specified render context. The
        /// workflow id is not bound into the address, because the editor sends it as a query parameter
        /// sourced from the state island.
        /// </summary>
        /// <param name="renderContext">
        /// The rendering context containing the request and parameters used to construct the URI.
        /// </param>
        /// <returns>
        /// An <see cref="IUri"/> representing the workflow editor resource. Returns null if the base URI
        /// cannot be resolved.
        /// </returns>
        private static IUri GetUri(IRenderControlContext renderContext)
        {
            return CoreHub.GetUri<global::KleeneStar.Core.WWW.Api._1_.Workflows.WorkflowEditor>()?
                .BindParameters(renderContext?.Request);
        }
    }
}
