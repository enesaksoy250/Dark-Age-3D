using DarkAge.Core.Domain;

namespace DarkAge.Gameplay.Results
{
    public sealed class CollectResourcesResult
    {
        public CollectResourcesResult(PlayerProgress playerProgress, ResourceWallet collectedResources)
        {
            PlayerProgress = playerProgress;
            CollectedResources = collectedResources;
        }

        public PlayerProgress PlayerProgress { get; }

        public ResourceWallet CollectedResources { get; }
    }
}
