using System;
using System.Diagnostics;
using Model = Discord.API.VoiceState;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct SocketVoiceState : IVoiceState
    {
        public static readonly SocketVoiceState Default = new SocketVoiceState(null, null, false, false, false, false, false);

        [Flags]
        private enum Flags : byte
        {
            Normal = 0x00,
            Suppressed = 0x01,
            Muted = 0x02,
            Deafened = 0x04,
            SelfMuted = 0x08,
            SelfDeafened = 0x10,
        }

        private readonly Flags _voiceStates;

        /// <summary>
        ///     Gets the voice channel that the user is currently in; or <see langword="null"/> if none.
        /// </summary>
        public SocketVoiceChannel VoiceChannel { get; }
        /// <inheritdoc />
        public string VoiceSessionId { get; }

        /// <inheritdoc />
        public bool IsMuted => (_voiceStates & Flags.Muted) != 0;
        /// <inheritdoc />
        public bool IsDeafened => (_voiceStates & Flags.Deafened) != 0;
        /// <inheritdoc />
        public bool IsSuppressed => (_voiceStates & Flags.Suppressed) != 0;
        /// <inheritdoc />
        public bool IsSelfMuted => (_voiceStates & Flags.SelfMuted) != 0;
        /// <inheritdoc />
        public bool IsSelfDeafened => (_voiceStates & Flags.SelfDeafened) != 0;

        internal SocketVoiceState(SocketVoiceChannel voiceChannel, string sessionId, bool isSelfMuted, bool isSelfDeafened, bool isMuted, bool isDeafened, bool isSuppressed)
        {
            VoiceChannel = voiceChannel;
            VoiceSessionId = sessionId;

            Flags voiceStates = Flags.Normal;
            if (isSelfMuted)
                voiceStates |= Flags.SelfMuted;
            if (isSelfDeafened)
                voiceStates |= Flags.SelfDeafened;
            if (isMuted)
                voiceStates |= Flags.Muted;
            if (isDeafened)
                voiceStates |= Flags.Deafened;
            if (isSuppressed)
                voiceStates |= Flags.Suppressed;
            _voiceStates = voiceStates;
        }
        internal static SocketVoiceState Create(SocketVoiceChannel voiceChannel, Model model)
        {
            return new SocketVoiceState(voiceChannel, model.SessionId, model.SelfMute, model.SelfDeaf, model.Mute, model.Deaf, model.Suppress);
        }

        /// <summary>
        ///     Gets the name of the voice channel.
        /// </summary>
        /// <returns>
        ///     The name of the voice channel.
        /// </returns>
        public override string ToString() => VoiceChannel?.Name ?? "Unknown";
        private string DebuggerDisplay => $"{VoiceChannel?.Name ?? "Unknown"} ({_voiceStates})";
        internal SocketVoiceState Clone() => this;

        IVoiceChannel IVoiceState.VoiceChannel => VoiceChannel;
    }
}
