using System;

namespace DarkAge.Core.Domain
{
    [Serializable]
    public sealed class ResourceProductionRule
    {
        public ResourceProductionRule(ResourceType resourceType, int amountPerMinute)
        {
            if (amountPerMinute < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amountPerMinute), "Amount per minute cannot be negative.");
            }

            ResourceType = resourceType;
            AmountPerMinute = amountPerMinute;
        }

        public ResourceType ResourceType { get; }

        public int AmountPerMinute { get; }
    }
}
