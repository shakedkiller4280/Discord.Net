using System;
using Model = Discord.API.InviteMetadata;

namespace Discord.Rest
{
    public class RestInviteMetadata : RestInvite, IInviteMetadata
    {
        private long? _createdAtTicks;

        public bool IsRevoked { get; private set; }
        public bool IsTemporary { get; private set; }
        public int? MaxAge { get; private set; }
        public int? MaxUses { get; private set; }
        public int? Uses { get; private set; }
        public RestUser Inviter { get; private set; }

        public DateTimeOffset? CreatedAt => DateTimeUtils.FromTicks(_createdAtTicks);

        internal RestInviteMetadata(BaseDiscordClient discord, IGuild guild, IChannel channel, string id)
            : base(discord, guild, channel, id)
        {
        }
        internal static RestInviteMetadata Create(BaseDiscordClient discord, IGuild guild, IChannel channel, Model model)
        {
            var entity = new RestInviteMetadata(discord, guild, channel, model.Code);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            base.Update(model);
            Inviter = model.Inviter != null ? RestUser.Create(Discord, model.Inviter) : null;
            IsRevoked = model.Revoked;
            IsTemporary = model.Temporary;
            MaxAge = model.MaxAge.IsSpecified ? model.MaxAge.Value : (int?)null;
            MaxUses = model.MaxUses.IsSpecified ? model.MaxUses.Value : (int?)null;
            Uses = model.Uses.IsSpecified ? model.Uses.Value : (int?)null;
            _createdAtTicks = model.CreatedAt.IsSpecified ? model.CreatedAt.Value.UtcTicks : (long?)null;
        }

        IUser IInviteMetadata.Inviter => Inviter;
    }
}
