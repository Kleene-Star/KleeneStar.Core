namespace KleeneStar.Core.WebImport
{
    /// <summary>
    /// Represents the connection and mapping settings for LDAP (Active Directory) imports.
    /// </summary>
    public class LdapImportSettings
    {
        /// <summary>
        /// Gets or sets the LDAP server hostname or IP address.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the LDAP server port. Default is 389 for LDAP, 636 for LDAPS.
        /// </summary>
        public int Port { get; set; } = 389;

        /// <summary>
        /// Gets or sets a value indicating whether to use SSL/TLS for the connection.
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// Gets or sets the distinguished name (DN) used to bind to the LDAP server.
        /// </summary>
        public string BindDn { get; set; }

        /// <summary>
        /// Gets or sets the password used for LDAP authentication.
        /// </summary>
        public string BindPassword { get; set; }

        /// <summary>
        /// Gets or sets the base DN from which to search for user entries.
        /// </summary>
        public string UserSearchBase { get; set; }

        /// <summary>
        /// Gets or sets the LDAP filter used to locate user entries.
        /// Default is "(objectClass=user)".
        /// </summary>
        public string UserFilter { get; set; } = "(objectClass=user)";

        /// <summary>
        /// Gets or sets the base DN from which to search for group entries.
        /// </summary>
        public string GroupSearchBase { get; set; }

        /// <summary>
        /// Gets or sets the LDAP filter used to locate group entries.
        /// Default is "(objectClass=group)".
        /// </summary>
        public string GroupFilter { get; set; } = "(objectClass=group)";

        /// <summary>
        /// Gets or sets the LDAP attribute that maps to the identity name.
        /// Default is "sAMAccountName".
        /// </summary>
        public string UserNameAttribute { get; set; } = "sAMAccountName";

        /// <summary>
        /// Gets or sets the LDAP attribute that maps to the identity email.
        /// Default is "mail".
        /// </summary>
        public string UserEmailAttribute { get; set; } = "mail";

        /// <summary>
        /// Gets or sets the LDAP attribute that maps to the group name.
        /// Default is "cn".
        /// </summary>
        public string GroupNameAttribute { get; set; } = "cn";

        /// <summary>
        /// Gets or sets the LDAP attribute that contains group member references.
        /// Default is "member".
        /// </summary>
        public string GroupMemberAttribute { get; set; } = "member";
    }
}
