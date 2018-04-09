using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord.API
{
    internal class AuditLog
    {
        [JsonProperty("webhooks")]
        public Webhook[] Webhooks { get; set; }

        [JsonProperty("webhooks")]
        public User[] Users { get; set; }

        [JsonProperty("webhooks")]
        public AuditLogEntry[] AuditLogEntries { get; set; }
    }

    internal class AuditLogEntry
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
        [JsonProperty("target_id")]
        public ulong? TargetId { get; set; }

        [JsonProperty("changes")]
        public Optional<AuditLogChange[]> Changes { get; set; }
        [JsonProperty("options")]
        public Optional<Options> Info { get; set; }
        [JsonProperty("action_type")]
        public ActionType Type { get; set; }

        [JsonProperty("reason")]
        public Optional<string> Reason { get; set; }
    }

    internal class Options
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        // Either 'member' or 'role'
        [JsonProperty("type")]
        public Optional<string> Type { get; set; }
        [JsonProperty("role_name")]
        public Optional<string> RoleName { get; set; }

        [JsonProperty("delete_member_days")]
        public Optional<int?> DeleteMemberDays { get; set; }
        [JsonProperty("members_removed")]
        public Optional<int?> MembersRemoved { get; set; }

        [JsonProperty("count")]
        public Optional<int?> Count { get; set; }
        [JsonProperty("channel_id")]
        public Optional<ulong?> ChannelId { get; set; }
    }

    internal enum ActionType
    {
        GuildUpdated = 1,

        ChannelCreated = 10,
        ChannelUpdated = 11,
        ChannelDeleted = 12,

        OverwriteCreated = 13,
        OverwriteUpdated = 14,
        OverwriteDeleted = 15,

        Kick = 20,
        Prune = 21,
        Ban = 22,
        Unban = 23,

        MemberUpdated = 24,
        MemberRoleUpdated = 25,

        RoleCreated = 30,
        RoleUpdated = 31,
        RoleDeleted = 32,

        InviteCreated = 40,
        InviteUpdated = 41,
        InviteDeleted = 42,

        WebhookCreated = 50,
        WebhookUpdated = 51,
        WebhookDeleted = 52,

        EmojiCreated = 60,
        EmojiUpdated = 61,
        EmojiDeleted = 62,

        MessageDeleted = 72
    }

    internal class AuditLogChange
    {
        [JsonProperty("key")]
        public string ChangedProperty { get; set; }

        [JsonProperty("new_value")]
        public JToken NewValue { get; set; }

        [JsonProperty("old_value")]
        public JToken OldValue { get; set; }
    }
}
