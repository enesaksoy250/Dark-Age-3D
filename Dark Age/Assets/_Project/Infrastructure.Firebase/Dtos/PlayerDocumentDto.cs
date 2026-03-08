using System;

namespace DarkAge.Infrastructure.Firebase.Dtos
{
    [Serializable]
    public sealed class PlayerDocumentDto
    {
        public string playerId;
        public string displayName;
        public long createdAtTicks;
        public long lastCollectedTicks;
        public ResourceValueDto[] resources;
        public BaseDocumentDto headquarters;
        public TaskDocumentDto[] tasks;
    }
}
