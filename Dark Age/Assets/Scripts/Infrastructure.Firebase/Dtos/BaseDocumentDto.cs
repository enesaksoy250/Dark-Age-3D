using System;

namespace DarkAge.Infrastructure.Firebase.Dtos
{
    [Serializable]
    public sealed class BaseDocumentDto
    {
        public string buildingType;
        public int gridX;
        public int gridZ;
        public long placedAtTicks;
    }
}
