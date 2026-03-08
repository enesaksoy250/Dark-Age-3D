using System.Collections.Generic;
using DarkAge.Core.Domain;

namespace DarkAge.Gameplay.Results
{
    public sealed class WorldStateSnapshot
    {
        public WorldStateSnapshot(PlayerProgress playerProgress, IReadOnlyList<WorldBaseRecord> worldBases)
        {
            PlayerProgress = playerProgress;
            WorldBases = worldBases;
        }

        public PlayerProgress PlayerProgress { get; }

        public IReadOnlyList<WorldBaseRecord> WorldBases { get; }
    }
}
