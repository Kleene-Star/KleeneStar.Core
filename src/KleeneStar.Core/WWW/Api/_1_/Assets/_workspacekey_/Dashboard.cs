using WebExpress.WebCore.WebAttribute;

namespace KleeneStar.Core.WWW.Api._1_.Assets._workspacekey_
{
    /// <summary>
    /// Dashboard endpoint of the asset overview's classic view. Reuses the object
    /// dashboard logic of
    /// <see cref="global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Dashboard"/>
    /// but aggregates the asset kind.
    /// </summary>
    [Title("kleenestar.core:object.view.dashboard.title")]
    [Cache]
    public sealed class Dashboard : global::KleeneStar.Core.WWW.Api._1_.Objects._workspacekey_.Dashboard
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Dashboard()
        {
        }

        /// <summary>
        /// Gets the object kind the dashboard aggregates: assets.
        /// </summary>
        protected override string Kind => Model.Entities.ObjectKind.Asset;
    }
}
