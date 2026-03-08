using System;
using System.Collections.Generic;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;

namespace DarkAge.Gameplay.Services
{
    public sealed class DefaultResourceTickService : IResourceTickService
    {
        public ResourceWallet CalculateProducedResources(DateTime fromUtc, DateTime toUtc, IReadOnlyList<ResourceProductionRule> rules)
        {
            var producedResources = ResourceWallet.Empty();
            if (toUtc <= fromUtc)
            {
                return producedResources;
            }

            var elapsedWholeMinutes = (int)Math.Floor((toUtc - fromUtc).TotalMinutes);
            if (elapsedWholeMinutes <= 0)
            {
                return producedResources;
            }

            foreach (var rule in rules)
            {
                producedResources.Add(rule.ResourceType, rule.AmountPerMinute * elapsedWholeMinutes);
            }

            return producedResources;
        }
    }
}
