using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;
using DarkAge.Gameplay.Results;

namespace DarkAge.Gameplay.UseCases
{
    public sealed class CollectProducedResourcesUseCase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IGameRulesProvider _gameRulesProvider;
        private readonly IResourceTickService _resourceTickService;
        private readonly ITimeProvider _timeProvider;
        private readonly EvaluateTasksUseCase _evaluateTasksUseCase;

        public CollectProducedResourcesUseCase(
            IPlayerRepository playerRepository,
            IGameRulesProvider gameRulesProvider,
            IResourceTickService resourceTickService,
            ITimeProvider timeProvider,
            EvaluateTasksUseCase evaluateTasksUseCase)
        {
            _playerRepository = playerRepository;
            _gameRulesProvider = gameRulesProvider;
            _resourceTickService = resourceTickService;
            _timeProvider = timeProvider;
            _evaluateTasksUseCase = evaluateTasksUseCase;
        }

        public async Task<UseCaseResult<CollectResourcesResult>> ExecuteAsync(PlayerId playerId, CancellationToken cancellationToken)
        {
            var playerProgress = await _playerRepository.LoadAsync(playerId, cancellationToken);
            if (playerProgress == null)
            {
                return UseCaseResult<CollectResourcesResult>.Failure("Player progress could not be loaded.");
            }

            var rules = _gameRulesProvider.GetRules();
            var currentTime = _timeProvider.UtcNow;
            var collectedResources = _resourceTickService.CalculateProducedResources(
                playerProgress.LastResourceCollectionUtc,
                currentTime,
                rules.ProductionRules);

            if (collectedResources.TotalAmount > 0)
            {
                playerProgress.ApplyCollectedResources(collectedResources, currentTime);
                await _evaluateTasksUseCase.ExecuteAsync(
                    playerProgress,
                    new Dictionary<TaskType, int>
                    {
                        { TaskType.CollectAnyResources, collectedResources.TotalAmount },
                        { TaskType.CollectFood, collectedResources.Get(ResourceType.Food) },
                        { TaskType.CollectGold, collectedResources.Get(ResourceType.Gold) }
                    },
                    cancellationToken);

                await _playerRepository.SaveAsync(playerProgress, cancellationToken);
            }

            return UseCaseResult<CollectResourcesResult>.Success(new CollectResourcesResult(playerProgress, collectedResources));
        }
    }
}
