namespace KleeneStar.Core.WebPermission
{
    /// <summary>
    /// Names the kinds of resource whose permissions can be administered.
    /// </summary>
    /// <remarks>
    /// A scope is stored with every grant and is also the prefix the policies of that resource
    /// carry in their registered name, so the dialog of a resource can offer the policies that
    /// apply to it instead of the whole catalog. Adding a further resource means adding a name
    /// here, not changing the schema.
    /// </remarks>
    public static class PermissionScope
    {
        /// <summary>
        /// The workspace and everything filed in it.
        /// </summary>
        public const string Workspace = "workspace";

        /// <summary>
        /// A single object, addressed by its object key.
        /// </summary>
        public const string Object = "object";

        /// <summary>
        /// A class of a workspace.
        /// </summary>
        public const string Class = "class";

        /// <summary>
        /// A dashboard.
        /// </summary>
        public const string Dashboard = "dashboard";

        /// <summary>
        /// A calendar.
        /// </summary>
        public const string Calendar = "calendar";
    }
}
