namespace KleeneStar.Core.Model.Workspace
{
    /// <summary>
    /// Specifies the state of a workspace type, indicating whether it is active or archived.
    /// </summary>
    public enum TypeWorkspaceState
    {
        /// <summary>
        /// Indicates whether the entity is currently active.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }
}
