using System;

namespace DarkAge.Infrastructure.Firebase.Dtos
{
    [Serializable]
    public sealed class WorldBaseDocumentDto
    {
        public string ownerId;
        public string ownerName;
        public BaseDocumentDto headquarters;
    }
}
