using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based channel in a guild that can send and receive messages.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketTextChannel : SocketGuildChannel, ITextChannel, ISocketMessageChannel
    {
        private readonly MessageCache _messages;

        /// <inheritdoc />
        public string Topic { get; private set; }

        private bool _nsfw;
        /// <inheritdoc />
        public bool IsNsfw => _nsfw || ChannelHelper.IsNsfw(this);

        /// <inheritdoc />
        public string Mention => MentionUtils.MentionChannel(Id);
        /// <inheritdoc />
        public IReadOnlyCollection<SocketMessage> CachedMessages => _messages?.Messages ?? ImmutableArray.Create<SocketMessage>();
        /// <inheritdoc />
        public override IReadOnlyCollection<SocketGuildUser> Users
            => Guild.Users.Where(x => Permissions.GetValue(
                Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
                ChannelPermission.ViewChannel)).ToImmutableArray();

        internal SocketTextChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id, guild)
        {
            if (Discord.MessageCacheSize > 0)
                _messages = new MessageCache(Discord, this);
        }
        internal new static SocketTextChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketTextChannel(guild.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }
        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);

            Topic = model.Topic.Value;
            _nsfw = model.Nsfw.GetValueOrDefault();
        }

        /// <inheritdoc />
        public Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null)
            => ChannelHelper.ModifyAsync(this, Discord, func, options);

        //Messages
        /// <inheritdoc />
        public SocketMessage GetCachedMessage(ulong id)
            => _messages?.Get(id);
        public async Task<IMessage> GetMessageAsync(ulong id, RequestOptions options = null)
        {
            IMessage msg = _messages?.Get(id);
            if (msg == null)
                msg = await ChannelHelper.GetMessageAsync(this, Discord, id, options).ConfigureAwait(false);
            return msg;
        }
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, null, Direction.Before, limit, CacheMode.AllowDownload, options);
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessageId, dir, limit, CacheMode.AllowDownload, options);
        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions options = null)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessage.Id, dir, limit, CacheMode.AllowDownload, options);
        /// <inheritdoc />
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, null, Direction.Before, limit);
        /// <inheritdoc />
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, fromMessageId, dir, limit);
        /// <inheritdoc />
        public IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => SocketChannelHelper.GetCachedMessages(this, Discord, _messages, fromMessage.Id, dir, limit);
        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
            => ChannelHelper.GetPinnedMessagesAsync(this, Discord, options);

        /// <inheritdoc />
        public Task<RestUserMessage> SendMessageAsync(string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ChannelHelper.SendMessageAsync(this, Discord, text, isTTS, embed, options);
#if FILESYSTEM
        /// <inheritdoc />
        public Task<RestUserMessage> SendFileAsync(string filePath, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ChannelHelper.SendFileAsync(this, Discord, filePath, text, isTTS, embed, options);
#endif
        /// <inheritdoc />
        public Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ChannelHelper.SendFileAsync(this, Discord, stream, filename, text, isTTS, embed, options);

        /// <inheritdoc />
        public Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null)
            => ChannelHelper.DeleteMessagesAsync(this, Discord, messages.Select(x => x.Id), options);
        /// <inheritdoc />
        public Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions options = null)
            => ChannelHelper.DeleteMessagesAsync(this, Discord, messageIds, options);

        /// <inheritdoc />
        public Task TriggerTypingAsync(RequestOptions options = null)
            => ChannelHelper.TriggerTypingAsync(this, Discord, options);
        public IDisposable EnterTypingState(RequestOptions options = null)
            => ChannelHelper.EnterTypingState(this, Discord, options);

        internal void AddMessage(SocketMessage msg)
            => _messages?.Add(msg);
        internal SocketMessage RemoveMessage(ulong id)
            => _messages?.Remove(id);

        //Users
        /// <inheritdoc />
        public override SocketGuildUser GetUser(ulong id)
        {
            var user = Guild.GetUser(id);
            if (user != null)
            {
                var guildPerms = Permissions.ResolveGuild(Guild, user);
                var channelPerms = Permissions.ResolveChannel(Guild, user, this, guildPerms);
                if (Permissions.GetValue(channelPerms, ChannelPermission.ViewChannel))
                    return user;
            }
            return null;
        }

        //Webhooks
        /// <summary>
        ///     Creates a webhook in this text channel.
        /// </summary>
        /// <param name="name">The name of the webhook.</param>
        /// <param name="avatar">The avatar of the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     The created webhook.
        /// </returns>
        public Task<RestWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null)
            => ChannelHelper.CreateWebhookAsync(this, Discord, name, avatar, options);
        /// <summary>
        ///     Gets the webhook in this text channel with the provided ID.
        /// </summary>
        /// <param name="id">The ID of the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A webhook associated with the <paramref name="id"/>, or <see langword="null"/> if not found.
        /// </returns>
        public Task<RestWebhook> GetWebhookAsync(ulong id, RequestOptions options = null)
            => ChannelHelper.GetWebhookAsync(this, Discord, id, options);
        /// <summary>
        ///     Gets the webhooks for this text channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A collection of webhooks.
        /// </returns>
        public Task<IReadOnlyCollection<RestWebhook>> GetWebhooksAsync(RequestOptions options = null)
            => ChannelHelper.GetWebhooksAsync(this, Discord, options);

        private string DebuggerDisplay => $"{Name} ({Id}, Text)";
        internal new SocketTextChannel Clone() => MemberwiseClone() as SocketTextChannel;

        //ITextChannel
        /// <inheritdoc />
        async Task<IWebhook> ITextChannel.CreateWebhookAsync(string name, Stream avatar, RequestOptions options)
            => await CreateWebhookAsync(name, avatar, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IWebhook> ITextChannel.GetWebhookAsync(ulong id, RequestOptions options)
            => await GetWebhookAsync(id, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IWebhook>> ITextChannel.GetWebhooksAsync(RequestOptions options)
            => await GetWebhooksAsync(options).ConfigureAwait(false);

        //IGuildChannel
        /// <inheritdoc />
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(GetUser(id));
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();

        //IMessageChannel
        /// <inheritdoc />
        async Task<IMessage> IMessageChannel.GetMessageAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetMessageAsync(id, options).ConfigureAwait(false);
            else
                return GetCachedMessage(id);
        }
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode, RequestOptions options)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, null, Direction.Before, limit, mode, options);
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessageId, dir, limit, mode, options);
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage fromMessage, Direction dir, int limit, CacheMode mode, RequestOptions options)
            => SocketChannelHelper.GetMessagesAsync(this, Discord, _messages, fromMessage.Id, dir, limit, mode, options);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IMessage>> IMessageChannel.GetPinnedMessagesAsync(RequestOptions options)
            => await GetPinnedMessagesAsync(options).ConfigureAwait(false);
#if FILESYSTEM
        /// <inheritdoc />
        async Task<IUserMessage> IMessageChannel.SendFileAsync(string filePath, string text, bool isTTS, Embed embed, RequestOptions options)
            => await SendFileAsync(filePath, text, isTTS, embed, options).ConfigureAwait(false);
#endif
        /// <inheritdoc />
        async Task<IUserMessage> IMessageChannel.SendFileAsync(Stream stream, string filename, string text, bool isTTS, Embed embed, RequestOptions options)
            => await SendFileAsync(stream, filename, text, isTTS, embed, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IUserMessage> IMessageChannel.SendMessageAsync(string text, bool isTTS, Embed embed, RequestOptions options)
            => await SendMessageAsync(text, isTTS, embed, options).ConfigureAwait(false);
        /// <inheritdoc />
        IDisposable IMessageChannel.EnterTypingState(RequestOptions options)
            => EnterTypingState(options);
    }
}
