namespace Discord
{
    /// <summary>
    ///     Defines how broadly the explicit content filter will apply to members of a guild.
    /// </summary>
    public enum ExplicitFilterLevel
    {
        /// <summary>
        ///     Do not scan any messages.
        /// </summary>
        Disabled = 0,
        /// <summary>
        ///     Scan messages from members without a role.
        /// </summary>
        MemberWithoutRoles = 1,
        /// <summary>
        ///     Scan messages sent by all members.
        /// </summary>
        AllMembers = 2
    }
}
