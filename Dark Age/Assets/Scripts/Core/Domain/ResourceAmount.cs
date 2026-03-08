using System;

namespace DarkAge.Core.Domain
{
    [Serializable]
    public readonly struct ResourceAmount
    {
        public ResourceAmount(ResourceType type, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
            }

            Type = type;
            Amount = amount;
        }

        public ResourceType Type { get; }

        public int Amount { get; }
    }
}
