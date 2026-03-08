using System;

namespace DarkAge.Core.Domain
{
    [Serializable]
    public sealed class PlayerProfile
    {
        public PlayerProfile(PlayerId playerId, string displayName, DateTime createdAtUtc)
        {
            PlayerId = playerId;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? "Unnamed Commander" : displayName;
            CreatedAtUtc = createdAtUtc;
        }

        public PlayerId PlayerId { get; }

        public string DisplayName { get; }

        public DateTime CreatedAtUtc { get; }
    }
}
