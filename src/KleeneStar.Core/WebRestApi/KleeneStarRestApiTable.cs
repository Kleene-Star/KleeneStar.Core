using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebIndex;

namespace KleeneStar.Core.WebRestApi
{
    /// <summary>
    /// Project-wide base class for REST API tables that persists the user-chosen
    /// column layout (order, width, visibility) to the <c>UserSession</c> store
    /// via <see cref="WebManager.ISessionManager"/>.
    ///
    /// Subclasses implement <see cref="RetrieveDefaultColumns"/> (their built-in
    /// column definitions). On every retrieve, the stored per-user layout — if
    /// any — is laid over the defaults; on every <c>POST/PUT /Configure</c> the
    /// new layout is persisted under the table's <see cref="System.Type.FullName"/>.
    /// </summary>
    /// <typeparam name="TIndexItem">Type of the index item.</typeparam>
    public abstract class KleeneStarRestApiTable<TIndexItem> : RestApiTable<TIndexItem>
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Returns a stable key used to address the per-user layout for this
        /// table. Defaults to the concrete type's full name so that different
        /// REST tables cannot collide.
        /// </summary>
        protected virtual string TableLayoutKey => GetType().FullName;

        /// <summary>
        /// Retrieves the column collection presented to the client. The default
        /// implementation calls <see cref="RetrieveDefaultColumns"/> and applies
        /// the layout previously stored for the current user (if any).
        /// </summary>
        /// <param name="request">The triggering request.</param>
        /// <returns>The effective column collection.</returns>
        protected override IEnumerable<RestApiTableColumn> RetrieveColums(IRequest request)
        {
            var defaults = RetrieveDefaultColumns(request);
            return CoreHub.SessionManager.ApplyStoredTableLayout(request, TableLayoutKey, defaults);
        }

        /// <summary>
        /// Persists the new layout under the current identity.
        /// </summary>
        /// <param name="columns">The reordered columns as resolved by the framework.</param>
        /// <param name="request">The triggering request.</param>
        protected override void UpdateColumns(IEnumerable<RestApiTableColumn> columns, IRequest request)
        {
            CoreHub.SessionManager.SetTableLayout(request, TableLayoutKey, columns);
        }

        /// <summary>
        /// Returns the built-in column definitions for the table. Subclasses
        /// implement this exactly as they previously implemented
        /// <c>RetrieveColums</c>; ordering / visibility / width here represent
        /// the default state shown to a user that has never customized the table.
        /// </summary>
        /// <param name="request">The triggering request.</param>
        /// <returns>The default columns.</returns>
        protected abstract IEnumerable<RestApiTableColumn> RetrieveDefaultColumns(IRequest request);
    }
}
