using System.Threading;
using System.Threading.Tasks;

namespace KleeneStar.Core.WebImport
{
    /// <summary>
    /// Defines the contract for importing identities and groups from external data sources.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface provide import capabilities from specific data
    /// sources (e.g., LDAP/Active Directory, CSV, SCIM). Each importer must support both
    /// initial migrations and recurring synchronization processes.
    /// </remarks>
    public interface IIdentityImporter
    {
        /// <summary>
        /// Gets the unique name identifying this importer type.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a human-readable description of the importer.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Validates the import settings and verifies connectivity to the external source.
        /// </summary>
        /// <param name="options">The import options containing connection settings.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// A task representing the asynchronous validation operation, returning an
        /// <see cref="IdentityImportResult"/> indicating whether the configuration is valid.
        /// </returns>
        Task<IdentityImportResult> ValidateAsync(IIdentityImportOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Imports identities and groups from the external source into the internal model.
        /// </summary>
        /// <param name="options">The import options containing connection and mapping settings.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// A task representing the asynchronous import operation, returning an
        /// <see cref="IdentityImportResult"/> containing the outcome of the import.
        /// </returns>
        Task<IdentityImportResult> ImportAsync(IIdentityImportOptions options, CancellationToken cancellationToken = default);
    }
}
