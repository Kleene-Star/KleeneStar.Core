using KleeneStar.Core.WebManager;
using KleeneStar.Model.Entities;
using System;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.WebImport
{
    /// <summary>
    /// Imports identities and groups from an LDAP (Active Directory) directory service.
    /// </summary>
    /// <remarks>
    /// This importer reads user and group entries from Active Directory, transforms the
    /// retrieved data, and imports it into the internal identity model. It supports both
    /// initial full migrations and recurring incremental synchronization processes.
    /// </remarks>
    public class LdapIdentityImporter : IIdentityImporter
    {
        private readonly IIdentityManager _identityManager;
        private readonly IGroupManager _groupManager;

        /// <summary>
        /// Gets the unique name identifying this importer.
        /// </summary>
        public string Name => "LDAP";

        /// <summary>
        /// Gets a human-readable description of the importer.
        /// </summary>
        public string Description => "Imports identities and groups from an LDAP (Active Directory) directory service.";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="identityManager">The identity manager for persisting imported identities.</param>
        /// <param name="groupManager">The group manager for persisting imported groups.</param>
        public LdapIdentityImporter(IIdentityManager identityManager, IGroupManager groupManager)
        {
            _identityManager = identityManager ?? throw new ArgumentNullException(nameof(identityManager));
            _groupManager = groupManager ?? throw new ArgumentNullException(nameof(groupManager));
        }

        /// <summary>
        /// Validates the LDAP connection settings by attempting to bind to the directory server.
        /// </summary>
        /// <param name="options">The import options containing LDAP connection settings.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A result indicating whether the connection settings are valid.</returns>
        public Task<IdentityImportResult> ValidateAsync(IdentityImportOptions options, CancellationToken cancellationToken = default)
        {
            var result = new IdentityImportResult();
            var ldap = options?.Ldap;

            if (ldap is null)
            {
                result.Errors.Add("LDAP settings are required.");
                return Task.FromResult(result);
            }

            if (string.IsNullOrWhiteSpace(ldap.Server))
            {
                result.Errors.Add("LDAP server address is required.");
                return Task.FromResult(result);
            }

            try
            {
                using var connection = CreateConnection(ldap);
                connection.Bind();
                result.Success = true;
            }
            catch (LdapException ex)
            {
                result.Errors.Add($"LDAP connection failed: {ex.Message}");
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// Imports identities and groups from the configured LDAP directory.
        /// </summary>
        /// <param name="options">The import options containing LDAP connection and mapping settings.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A result containing the import statistics and any errors.</returns>
        public Task<IdentityImportResult> ImportAsync(IdentityImportOptions options, CancellationToken cancellationToken = default)
        {
            var result = new IdentityImportResult();
            var ldap = options?.Ldap;

            if (ldap is null)
            {
                result.Errors.Add("LDAP settings are required.");
                return Task.FromResult(result);
            }

            try
            {
                using var connection = CreateConnection(ldap);
                connection.Bind();

                if (!string.IsNullOrWhiteSpace(ldap.UserSearchBase))
                {
                    ImportIdentities(connection, ldap, result, options.FullSync, cancellationToken);
                }

                if (!string.IsNullOrWhiteSpace(ldap.GroupSearchBase))
                {
                    ImportGroups(connection, ldap, result, options.FullSync, cancellationToken);
                }

                result.Success = result.Errors.Count == 0;
            }
            catch (LdapException ex)
            {
                result.Errors.Add($"LDAP import failed: {ex.Message}");
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// Creates an LDAP connection from the provided settings.
        /// </summary>
        private static LdapConnection CreateConnection(LdapImportSettings settings)
        {
            var directoryIdentifier = new LdapDirectoryIdentifier(settings.Server, settings.Port);
            var credential = new NetworkCredential(settings.BindDn, settings.BindPassword);
            var connection = new LdapConnection(directoryIdentifier, credential);

            if (settings.UseSsl)
            {
                connection.SessionOptions.SecureSocketLayer = true;
            }

            connection.AuthType = AuthType.Basic;

            return connection;
        }

        /// <summary>
        /// Imports user entries from the LDAP directory as identities.
        /// </summary>
        private void ImportIdentities(LdapConnection connection, LdapImportSettings settings, IdentityImportResult result, bool fullSync, CancellationToken cancellationToken)
        {
            var searchRequest = new SearchRequest(
                settings.UserSearchBase,
                settings.UserFilter,
                SearchScope.Subtree,
                settings.UserNameAttribute, settings.UserEmailAttribute
            );

            var response = (SearchResponse)connection.SendRequest(searchRequest);

            foreach (SearchResultEntry entry in response.Entries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var name = GetAttributeValue(entry, settings.UserNameAttribute);
                var email = GetAttributeValue(entry, settings.UserEmailAttribute);

                if (string.IsNullOrWhiteSpace(name))
                {
                    result.Warnings.Add($"Skipped entry with empty name: {entry.DistinguishedName}");
                    continue;
                }

                try
                {
                    var query = new Query<Identity>()
                        .WhereEqualsIgnoreCase(x => x.Name, name)
                        .WithPaging(0, 1);

                    var existing = _identityManager.GetIdentities(query).FirstOrDefault();

                    if (existing is not null)
                    {
                        existing.Email = email ?? existing.Email;
                        _identityManager.Update(existing);
                        result.IdentitiesUpdated++;
                    }
                    else
                    {
                        var identity = new Identity(Guid.NewGuid())
                        {
                            Name = name,
                            Email = email,
                            State = IdentityState.Active,
                            Icon = CoreHub.GenerateIcon(Guid.NewGuid())
                        };

                        _identityManager.Add(identity);
                        result.IdentitiesCreated++;
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Failed to import identity '{name}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Imports group entries from the LDAP directory as groups.
        /// </summary>
        private void ImportGroups(LdapConnection connection, LdapImportSettings settings, IdentityImportResult result, bool fullSync, CancellationToken cancellationToken)
        {
            var searchRequest = new SearchRequest(
                settings.GroupSearchBase,
                settings.GroupFilter,
                SearchScope.Subtree,
                settings.GroupNameAttribute
            );

            var response = (SearchResponse)connection.SendRequest(searchRequest);

            foreach (SearchResultEntry entry in response.Entries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var name = GetAttributeValue(entry, settings.GroupNameAttribute);

                if (string.IsNullOrWhiteSpace(name))
                {
                    result.Warnings.Add($"Skipped group with empty name: {entry.DistinguishedName}");
                    continue;
                }

                try
                {
                    var query = new Query<Group>()
                        .WhereEqualsIgnoreCase(x => x.Name, name)
                        .WithPaging(0, 1);

                    var existing = _groupManager.GetGroups(query).FirstOrDefault();

                    if (existing is not null)
                    {
                        _groupManager.Update(existing);
                        result.GroupsUpdated++;
                    }
                    else
                    {
                        var group = new Group(Guid.NewGuid())
                        {
                            Name = name,
                            State = GroupState.Active,
                            Icon = CoreHub.GenerateIcon(Guid.NewGuid())
                        };

                        _groupManager.Add(group);
                        result.GroupsCreated++;
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Failed to import group '{name}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Retrieves a single string value from an LDAP directory entry attribute.
        /// </summary>
        private static string GetAttributeValue(SearchResultEntry entry, string attributeName)
        {
            var attribute = entry.Attributes[attributeName];

            if (attribute is null || attribute.Count == 0)
            {
                return null;
            }

            return attribute[0]?.ToString();
        }
    }
}
