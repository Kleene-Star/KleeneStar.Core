using System.Collections.Generic;

namespace KleeneStar.Core.WebImport
{
    /// <summary>
    /// Represents the result of an identity import operation.
    /// </summary>
    public class IdentityImportResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the number of identities that were created.
        /// </summary>
        public int IdentitiesCreated { get; set; }

        /// <summary>
        /// Gets or sets the number of identities that were updated.
        /// </summary>
        public int IdentitiesUpdated { get; set; }

        /// <summary>
        /// Gets or sets the number of groups that were created.
        /// </summary>
        public int GroupsCreated { get; set; }

        /// <summary>
        /// Gets or sets the number of groups that were updated.
        /// </summary>
        public int GroupsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of error messages encountered during the import.
        /// </summary>
        public IList<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the collection of warning messages encountered during the import.
        /// </summary>
        public IList<string> Warnings { get; set; } = new List<string>();
    }
}
