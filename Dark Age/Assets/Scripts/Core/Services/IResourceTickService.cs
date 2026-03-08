using System;
using System.Collections.Generic;
using DarkAge.Core.Domain;

namespace DarkAge.Core.Services
{
    public interface IResourceTickService
    {
        ResourceWallet CalculateProducedResources(DateTime fromUtc, DateTime toUtc, IReadOnlyList<ResourceProductionRule> rules);
    }
}
