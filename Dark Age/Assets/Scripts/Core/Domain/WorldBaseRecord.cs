using System;

namespace DarkAge.Core.Domain
{
    [Serializable]
    public sealed class WorldBaseRecord
    {
        public WorldBaseRecord(PlayerId ownerId, string ownerName, BaseState baseState)
        {
            OwnerId = ownerId;
            OwnerName = ownerName;
            BaseState = baseState;
        }

        public PlayerId OwnerId { get; }

        public string OwnerName { get; }

        public BaseState BaseState { get; }
    }
}
