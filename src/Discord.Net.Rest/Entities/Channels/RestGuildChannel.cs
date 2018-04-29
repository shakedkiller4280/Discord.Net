using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a private REST group channel.
    /// </summary>
    public class RestGuildChannel : RestChannel, IGuildChannel
    {
        private ImmutableArray<Overwrite> _overwrites;

        /// <inheritdoc />
        public IReadOnlyCollection<Overwrite> PermissionOverwrites => _overwrites;

        internal IGuild Guild { get; }
        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public int Position { get; private set; }
        /// <inheritdoc />
        public ulong? CategoryId { get; private set; }
        /// <inheritdoc />
        public ulong GuildId => Guild.Id;

        internal RestGuildChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, id)
        {
            Guild = guild;
        }
        internal static RestGuildChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                    return RestTextChannel.Create(discord, guild, model);
                case ChannelType.Voice:
                    return RestVoiceChannel.Create(discord, guild, model);
                case ChannelType.Category:
                    return RestCategoryChannel.Create(discord, guild, model);
                default:
                    // TODO: Channel categories
                    return new RestGuildChannel(discord, guild, model.Id);
            }
        }
        internal override void Update(Model model)
        {
            Name = model.Name.Value;
            Position = model.Position.Value;

            var overwrites = model.PermissionOverwrites.Value;
            var newOverwrites = ImmutableArray.CreateBuilder<Overwrite>(overwrites.Length);
            for (int i = 0; i < overwrites.Length; i++)
                newOverwrites.Add(overwrites[i].ToEntity());
            _overwrites = newOverwrites.ToImmutable();
        }

        /// <inheritdoc />
        public override async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetChannelAsync(GuildId, Id, options).ConfigureAwait(false);
            Update(model);
        }
        /// <inheritdoc />
        public async Task ModifyAsync(Action<GuildChannelProperties> func, RequestOptions options = null)
        {
            var model = await ChannelHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }
        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => ChannelHelper.DeleteAsync(this, Discord, options);

        /// <inheritdoc />
        public async Task<ICategoryChannel> GetCategoryAsync()
        {
            if (CategoryId.HasValue)
                return (await Guild.GetChannelAsync(CategoryId.Value).ConfigureAwait(false)) as ICategoryChannel;
            return null;
        }

        public OverwritePermissions? GetPermissionOverwrite(IUser user)
        {
            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == user.Id)
                    return _overwrites[i].Permissions;
            }
            return null;
        }
        public OverwritePermissions? GetPermissionOverwrite(IRole role)
        {
            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == role.Id)
                    return _overwrites[i].Permissions;
            }
            return null;
        }
        public async Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions perms, RequestOptions options = null)
        {
            await ChannelHelper.AddPermissionOverwriteAsync(this, Discord, user, perms, options).ConfigureAwait(false);
            _overwrites = _overwrites.Add(new Overwrite(user.Id, PermissionTarget.User, new OverwritePermissions(perms.AllowValue, perms.DenyValue)));
        }
        public async Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions perms, RequestOptions options = null)
        {
            await ChannelHelper.AddPermissionOverwriteAsync(this, Discord, role, perms, options).ConfigureAwait(false);
            _overwrites = _overwrites.Add(new Overwrite(role.Id, PermissionTarget.Role, new OverwritePermissions(perms.AllowValue, perms.DenyValue)));
        }
        public async Task RemovePermissionOverwriteAsync(IUser user, RequestOptions options = null)
        {
            await ChannelHelper.RemovePermissionOverwriteAsync(this, Discord, user, options).ConfigureAwait(false);

            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == user.Id)
                {
                    _overwrites = _overwrites.RemoveAt(i);
                    return;
                }
            }
        }
        public async Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null)
        {
            await ChannelHelper.RemovePermissionOverwriteAsync(this, Discord, role, options).ConfigureAwait(false);

            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == role.Id)
                {
                    _overwrites = _overwrites.RemoveAt(i);
                    return;
                }
            }
        }

        public async Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
            => await ChannelHelper.GetInvitesAsync(this, Discord, options).ConfigureAwait(false);
        public async Task<RestInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, options).ConfigureAwait(false);

        public override string ToString() => Name;

        //IGuildChannel
        /// <inheritdoc />
        IGuild IGuildChannel.Guild
        {
            get
            {
                if (Guild != null)
                    return Guild;
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IInviteMetadata>> IGuildChannel.GetInvitesAsync(RequestOptions options)
            => await GetInvitesAsync(options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IInviteMetadata> IGuildChannel.CreateInviteAsync(int? maxAge, int? maxUses, bool isTemporary, bool isUnique, RequestOptions options)
            => await CreateInviteAsync(maxAge, maxUses, isTemporary, isUnique, options).ConfigureAwait(false);

        /// <inheritdoc />
        OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IRole role)
            => GetPermissionOverwrite(role);
        /// <inheritdoc />
        OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IUser user)
            => GetPermissionOverwrite(user);
        /// <inheritdoc />
        async Task IGuildChannel.AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions options)
            => await AddPermissionOverwriteAsync(role, permissions, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task IGuildChannel.AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions options)
            => await AddPermissionOverwriteAsync(user, permissions, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task IGuildChannel.RemovePermissionOverwriteAsync(IRole role, RequestOptions options)
            => await RemovePermissionOverwriteAsync(role, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task IGuildChannel.RemovePermissionOverwriteAsync(IUser user, RequestOptions options)
            => await RemovePermissionOverwriteAsync(user, options).ConfigureAwait(false);

        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>(); //Overridden //Overridden in Text/Voice
        /// <inheritdoc />
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(null); //Overridden in Text/Voice

        //IChannel
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden in Text/Voice
        /// <inheritdoc />
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(null); //Overridden in Text/Voice
    }
}
