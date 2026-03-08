using System;

namespace DarkAge.Infrastructure.Firebase.Dtos
{
    [Serializable]
    public sealed class WorldBaseCollectionDto
    {
        public WorldBaseDocumentDto[] bases;
    }
}
