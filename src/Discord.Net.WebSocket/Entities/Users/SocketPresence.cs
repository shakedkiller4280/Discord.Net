using System.Diagnostics;
using Model = Discord.API.Presence;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct SocketPresence : IPresence
    {
        /// <inheritdoc />
        public UserStatus Status { get; }
        /// <inheritdoc />
        public IActivity Activity { get; }

        internal SocketPresence(UserStatus status, IActivity activity)
        {
            Status = status;
            Activity= activity;
        }
        internal static SocketPresence Create(Model model)
        {
            return new SocketPresence(model.Status, model.Game?.ToEntity());
        }

        public override string ToString() => Status.ToString();
        private string DebuggerDisplay => $"{Status}{(Activity != null ? $", {Activity.Name}": "")}";

        internal SocketPresence Clone() => this;
    }
}
