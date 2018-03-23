using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    /// Requires the user invoking the command to have a specified permission.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireUserPermissionAttribute : PreconditionAttribute
    {
        public GuildPermission? GuildPermission { get; }
        public ChannelPermission? ChannelPermission { get; }

        /// <summary>
        /// Requires that the user invoking the command to have a specific <see cref="GuildPermission"/>.
        /// </summary>
        /// <remarks>This precondition will always fail if the command is being invoked in a private channel.</remarks>
        /// <param name="permission">The GuildPermission that the user must have. Multiple permissions can be specified by ORing the permissions together.</param>
        public RequireUserPermissionAttribute(GuildPermission permission)
        {
            GuildPermission = permission;
            ChannelPermission = null;
        }
        /// <summary>
        /// Requires that the user invoking the command to have a specific <see cref="ChannelPermission"/>.
        /// </summary>
        /// <param name="permission">The ChannelPermission that the user must have. Multiple permissions can be specified by ORing the permissions together.</param>
        /// <example>
        /// <code language="c#">
        ///     [Command("permission")]
        ///     [RequireUserPermission(ChannelPermission.ReadMessageHistory | ChannelPermission.ReadMessages)]
        ///     public async Task HasPermission()
        ///     {
        ///         await ReplyAsync("You can read messages and the message history!");
        ///     }
        /// </code>
        /// </example>
        public RequireUserPermissionAttribute(ChannelPermission permission)
        {
            ChannelPermission = permission;
            GuildPermission = null;
        }
        
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guildUser = context.User as IGuildUser;

            if (GuildPermission.HasValue)
            {
                if (guildUser == null)
                    return Task.FromResult(PreconditionResult.FromError("Command must be used in a guild channel"));                
                if (!guildUser.GuildPermissions.Has(GuildPermission.Value))
                    return Task.FromResult(PreconditionResult.FromError($"User requires guild permission {GuildPermission.Value}"));
            }

            if (ChannelPermission.HasValue)
            {
                ChannelPermissions perms;
                if (context.Channel is IGuildChannel guildChannel)
                    perms = guildUser.GetPermissions(guildChannel);
                else
                    perms = ChannelPermissions.All(context.Channel);

                if (!perms.Has(ChannelPermission.Value))
                    return Task.FromResult(PreconditionResult.FromError($"User requires channel permission {ChannelPermission.Value}"));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
