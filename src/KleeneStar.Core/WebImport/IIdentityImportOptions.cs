namespace KleeneStar.Core.WebImport
{
    /// <summary>
    /// Represents the options for an identity import operation.
    /// </summary>
    public interface IIdentityImportOptions
    {
        /// <summary>
        /// Gets a value indicating whether this is a full initial migration
        /// or an incremental synchronization.
        /// </summary>
        public bool FullSync { get; }

        /// <summary>
        /// Gets the specific import settings. Only applicable when
        /// using the importer.
        /// </summary>
        public IImportSettings Ldap { get; }
    }
}
