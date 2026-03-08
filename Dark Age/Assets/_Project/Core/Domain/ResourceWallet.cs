using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkAge.Core.Domain
{
    [Serializable]
    public sealed class ResourceWallet
    {
        private readonly Dictionary<ResourceType, int> _amounts;

        public ResourceWallet()
        {
            _amounts = CreateEmptyDictionary();
        }

        public ResourceWallet(IEnumerable<ResourceAmount> amounts)
            : this()
        {
            foreach (var amount in amounts)
            {
                Set(amount.Type, amount.Amount);
            }
        }

        public int TotalAmount => _amounts.Values.Sum();

        public int Get(ResourceType resourceType)
        {
            return _amounts.TryGetValue(resourceType, out var value) ? value : 0;
        }

        public void Set(ResourceType resourceType, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
            }

            _amounts[resourceType] = amount;
        }

        public void Add(ResourceType resourceType, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Added amount cannot be negative.");
            }

            _amounts[resourceType] = Get(resourceType) + amount;
        }

        public void Add(ResourceWallet other)
        {
            foreach (var amount in other.AsReadOnlyList())
            {
                Add(amount.Type, amount.Amount);
            }
        }

        public bool CanAfford(IEnumerable<ResourceAmount> costs)
        {
            return costs.All(cost => Get(cost.Type) >= cost.Amount);
        }

        public ResourceWallet Clone()
        {
            return new ResourceWallet(AsReadOnlyList());
        }

        public IReadOnlyList<ResourceAmount> AsReadOnlyList()
        {
            return _amounts
                .OrderBy(pair => (int)pair.Key)
                .Select(pair => new ResourceAmount(pair.Key, pair.Value))
                .ToArray();
        }

        public static ResourceWallet Empty()
        {
            return new ResourceWallet();
        }

        private static Dictionary<ResourceType, int> CreateEmptyDictionary()
        {
            var dictionary = new Dictionary<ResourceType, int>();
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                dictionary[resourceType] = 0;
            }

            return dictionary;
        }
    }
}
