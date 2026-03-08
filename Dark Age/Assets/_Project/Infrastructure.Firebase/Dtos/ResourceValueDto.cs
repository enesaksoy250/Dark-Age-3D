using System;

namespace DarkAge.Infrastructure.Firebase.Dtos
{
    [Serializable]
    public sealed class ResourceValueDto
    {
        public string resourceType;
        public int amount;
    }
}
