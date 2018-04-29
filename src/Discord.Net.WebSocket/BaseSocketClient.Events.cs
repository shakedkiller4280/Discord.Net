using System;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public partial class BaseSocketClient
    {
        //Channels
        /// <summary> Fired when a channel is created. </summary>
        public event Func<SocketChannel, Task> ChannelCreated 
        {
            add { _channelCreatedEvent.Add(value); }
            remove { _channelCreatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketChannel, Task>> _channelCreatedEvent = new AsyncEvent<Func<SocketChannel, Task>>();
        /// <summary> Fired when a channel is destroyed. </summary>
        public event Func<SocketChannel, Task> ChannelDestroyed {
            add { _channelDestroyedEvent.Add(value); }
            remove { _channelDestroyedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketChannel, Task>> _channelDestroyedEvent = new AsyncEvent<Func<SocketChannel, Task>>();
        /// <summary> Fired when a channel is updated. </summary>
        public event Func<SocketChannel, SocketChannel, Task> ChannelUpdated {
            add { _channelUpdatedEvent.Add(value); }
            remove { _channelUpdatedEvent.Remove(value); }
        }    
        internal readonly AsyncEvent<Func<SocketChannel, SocketChannel, Task>> _channelUpdatedEvent = new AsyncEvent<Func<SocketChannel, SocketChannel, Task>>();

        //Messages
        /// <summary> Fired when a message is received. </summary>
        public event Func<SocketMessage, Task> MessageReceived {
            add { _messageReceivedEvent.Add(value); }
            remove { _messageReceivedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketMessage, Task>> _messageReceivedEvent = new AsyncEvent<Func<SocketMessage, Task>>();
        /// <summary> Fired when a message is deleted. </summary>
        public event Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted {
            add { _messageDeletedEvent.Add(value); }
            remove { _messageDeletedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task>> _messageDeletedEvent = new AsyncEvent<Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task>>();
        /// <summary> Fired when a message is updated. </summary>
        public event Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated {
            add { _messageUpdatedEvent.Add(value); }
            remove { _messageUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task>> _messageUpdatedEvent = new AsyncEvent<Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task>>();
        /// <summary> Fired when a reaction is added to a message. </summary>
        public event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded {
            add { _reactionAddedEvent.Add(value); }
            remove { _reactionAddedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>> _reactionAddedEvent = new AsyncEvent<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>>();
        /// <summary> Fired when a reaction is removed from a message. </summary>
        public event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved {
            add { _reactionRemovedEvent.Add(value); }
            remove { _reactionRemovedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>> _reactionRemovedEvent = new AsyncEvent<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>>();
        /// <summary> Fired when all reactions to a message are cleared. </summary>
        public event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared {
            add { _reactionsClearedEvent.Add(value); }
            remove { _reactionsClearedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task>> _reactionsClearedEvent = new AsyncEvent<Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task>>();

        //Roles
        /// <summary> Fired when a role is created. </summary>
        public event Func<SocketRole, Task> RoleCreated {
            add { _roleCreatedEvent.Add(value); }
            remove { _roleCreatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketRole, Task>> _roleCreatedEvent = new AsyncEvent<Func<SocketRole, Task>>();
        /// <summary> Fired when a role is deleted. </summary>
        public event Func<SocketRole, Task> RoleDeleted {
            add { _roleDeletedEvent.Add(value); }
            remove { _roleDeletedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketRole, Task>> _roleDeletedEvent = new AsyncEvent<Func<SocketRole, Task>>();
        /// <summary> Fired when a role is updated. </summary>
        public event Func<SocketRole, SocketRole, Task> RoleUpdated {
            add { _roleUpdatedEvent.Add(value); }
            remove { _roleUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketRole, SocketRole, Task>> _roleUpdatedEvent = new AsyncEvent<Func<SocketRole, SocketRole, Task>>();

        //Guilds
        /// <summary> Fired when the connected account joins a guild. </summary>
        public event Func<SocketGuild, Task> JoinedGuild {
            add { _joinedGuildEvent.Add(value); }
            remove { _joinedGuildEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _joinedGuildEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when the connected account leaves a guild. </summary>
        public event Func<SocketGuild, Task> LeftGuild {
            add { _leftGuildEvent.Add(value); }
            remove { _leftGuildEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _leftGuildEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when a guild becomes available. </summary>
        public event Func<SocketGuild, Task> GuildAvailable {
            add { _guildAvailableEvent.Add(value); }
            remove { _guildAvailableEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildAvailableEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when a guild becomes unavailable. </summary>
        public event Func<SocketGuild, Task> GuildUnavailable {
            add { _guildUnavailableEvent.Add(value); }
            remove { _guildUnavailableEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildUnavailableEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when offline guild members are downloaded. </summary>
        public event Func<SocketGuild, Task> GuildMembersDownloaded {
            add { _guildMembersDownloadedEvent.Add(value); }
            remove { _guildMembersDownloadedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, Task>> _guildMembersDownloadedEvent = new AsyncEvent<Func<SocketGuild, Task>>();
        /// <summary> Fired when a guild is updated. </summary>
        public event Func<SocketGuild, SocketGuild, Task> GuildUpdated {
            add { _guildUpdatedEvent.Add(value); }
            remove { _guildUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuild, SocketGuild, Task>> _guildUpdatedEvent = new AsyncEvent<Func<SocketGuild, SocketGuild, Task>>();

        //Users
        /// <summary> Fired when a user joins a guild. </summary>
        public event Func<SocketGuildUser, Task> UserJoined {
            add { _userJoinedEvent.Add(value); }
            remove { _userJoinedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuildUser, Task>> _userJoinedEvent = new AsyncEvent<Func<SocketGuildUser, Task>>();
        /// <summary> Fired when a user leaves a guild. </summary>
        public event Func<SocketGuildUser, Task> UserLeft {
            add { _userLeftEvent.Add(value); }
            remove { _userLeftEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuildUser, Task>> _userLeftEvent = new AsyncEvent<Func<SocketGuildUser, Task>>();
        /// <summary> Fired when a user is banned from a guild. </summary>
        public event Func<SocketUser, SocketGuild, Task> UserBanned {
            add { _userBannedEvent.Add(value); }
            remove { _userBannedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketUser, SocketGuild, Task>> _userBannedEvent = new AsyncEvent<Func<SocketUser, SocketGuild, Task>>();
        /// <summary> Fired when a user is unbanned from a guild. </summary>
        public event Func<SocketUser, SocketGuild, Task> UserUnbanned {
            add { _userUnbannedEvent.Add(value); }
            remove { _userUnbannedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketUser, SocketGuild, Task>> _userUnbannedEvent = new AsyncEvent<Func<SocketUser, SocketGuild, Task>>();
        /// <summary> Fired when a user is updated. </summary>
        public event Func<SocketUser, SocketUser, Task> UserUpdated {
            add { _userUpdatedEvent.Add(value); }
            remove { _userUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketUser, SocketUser, Task>> _userUpdatedEvent = new AsyncEvent<Func<SocketUser, SocketUser, Task>>();
        /// <summary> Fired when a guild member is updated, or a member presence is updated. </summary>
        public event Func<SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated {
            add { _guildMemberUpdatedEvent.Add(value); }
            remove { _guildMemberUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGuildUser, SocketGuildUser, Task>> _guildMemberUpdatedEvent = new AsyncEvent<Func<SocketGuildUser, SocketGuildUser, Task>>();     
        /// <summary> Fired when a user joins, leaves, or moves voice channels. </summary>
        public event Func<SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated {
            add { _userVoiceStateUpdatedEvent.Add(value); }
            remove { _userVoiceStateUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketUser, SocketVoiceState, SocketVoiceState, Task>> _userVoiceStateUpdatedEvent = new AsyncEvent<Func<SocketUser, SocketVoiceState, SocketVoiceState, Task>>();
        /// <summary> Fired when the connected account is updated. </summary>
        public event Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated {
            add { _selfUpdatedEvent.Add(value); }
            remove { _selfUpdatedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketSelfUser, SocketSelfUser, Task>> _selfUpdatedEvent = new AsyncEvent<Func<SocketSelfUser, SocketSelfUser, Task>>();
        /// <summary> Fired when a user starts typing. </summary>
        public event Func<SocketUser, ISocketMessageChannel, Task> UserIsTyping {
            add { _userIsTypingEvent.Add(value); }
            remove { _userIsTypingEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketUser, ISocketMessageChannel, Task>> _userIsTypingEvent = new AsyncEvent<Func<SocketUser, ISocketMessageChannel, Task>>();
        /// <summary> Fired when a user joins a group channel. </summary>
        public event Func<SocketGroupUser, Task> RecipientAdded {
            add { _recipientAddedEvent.Add(value); }
            remove { _recipientAddedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGroupUser, Task>> _recipientAddedEvent = new AsyncEvent<Func<SocketGroupUser, Task>>();
        /// <summary> Fired when a user is removed from a group channel. </summary>
        public event Func<SocketGroupUser, Task> RecipientRemoved {
            add { _recipientRemovedEvent.Add(value); }
            remove { _recipientRemovedEvent.Remove(value); }
        }
        internal readonly AsyncEvent<Func<SocketGroupUser, Task>> _recipientRemovedEvent = new AsyncEvent<Func<SocketGroupUser, Task>>();
    }
}
