using System.Collections.Generic;

namespace DarkAge.Core.Domain
{
    public sealed class GameRules
    {
        public GameRules(
            ResourceWallet startingResources,
            IReadOnlyList<ResourceProductionRule> productionRules,
            IReadOnlyList<TaskDefinition> taskDefinitions,
            WorldBounds worldBounds,
            float gridCellSize)
        {
            StartingResources = startingResources.Clone();
            ProductionRules = productionRules;
            TaskDefinitions = taskDefinitions;
            WorldBounds = worldBounds;
            GridCellSize = gridCellSize;
        }

        public ResourceWallet StartingResources { get; }

        public IReadOnlyList<ResourceProductionRule> ProductionRules { get; }

        public IReadOnlyList<TaskDefinition> TaskDefinitions { get; }

        public WorldBounds WorldBounds { get; }

        public float GridCellSize { get; }
    }
}
