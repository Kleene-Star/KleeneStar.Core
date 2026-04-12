namespace KleeneStar.Core.WebImport
{
    /// <summary>
    /// Represents the options for an identity import operation.
    /// </summary>
    public class IdentityImportOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether this is a full initial migration
        /// or an incremental synchronization.
        /// </summary>
        public bool FullSync { get; set; } = true;

        /// <summary>
        /// Gets or sets the LDAP-specific import settings. Only applicable when
        /// using the LDAP importer.
        /// </summary>
        public LdapImportSettings Ldap { get; set; }
    }
}
