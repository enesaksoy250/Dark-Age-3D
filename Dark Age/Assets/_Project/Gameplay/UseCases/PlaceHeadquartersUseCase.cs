using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;
using DarkAge.Gameplay.Results;

namespace DarkAge.Gameplay.UseCases
{
    public sealed class PlaceHeadquartersUseCase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IWorldRepository _worldRepository;
        private readonly IGameRulesProvider _gameRulesProvider;
        private readonly ITimeProvider _timeProvider;
        private readonly EvaluateTasksUseCase _evaluateTasksUseCase;

        public PlaceHeadquartersUseCase(
            IPlayerRepository playerRepository,
            IWorldRepository worldRepository,
            IGameRulesProvider gameRulesProvider,
            ITimeProvider timeProvider,
            EvaluateTasksUseCase evaluateTasksUseCase)
        {
            _playerRepository = playerRepository;
            _worldRepository = worldRepository;
            _gameRulesProvider = gameRulesProvider;
            _timeProvider = timeProvider;
            _evaluateTasksUseCase = evaluateTasksUseCase;
        }

        public async Task<UseCaseResult<PlayerProgress>> ExecuteAsync(PlayerId playerId, GridPosition gridPosition, CancellationToken cancellationToken)
        {
            var playerProgress = await _playerRepository.LoadAsync(playerId, cancellationToken);
            if (playerProgress == null)
            {
                return UseCaseResult<PlayerProgress>.Failure("Player progress could not be loaded.");
            }

            if (playerProgress.HasHeadquarters)
            {
                return UseCaseResult<PlayerProgress>.Failure("Headquarters already placed.");
            }

            var rules = _gameRulesProvider.GetRules();
            if (!rules.WorldBounds.Contains(gridPosition))
            {
                return UseCaseResult<PlayerProgress>.Failure("Selected grid position is outside world bounds.");
            }

            if (await _worldRepository.IsGridOccupiedAsync(gridPosition, cancellationToken))
            {
                return UseCaseResult<PlayerProgress>.Failure("Selected grid position is already occupied.");
            }

            var headquarters = new BaseState(BuildingType.Headquarters, gridPosition, _timeProvider.UtcNow);
            playerProgress.PlaceHeadquarters(headquarters);

            await _worldRepository.SaveBaseAsync(
                new WorldBaseRecord(playerProgress.Profile.PlayerId, playerProgress.Profile.DisplayName, headquarters),
                cancellationToken);

            await _evaluateTasksUseCase.ExecuteAsync(
                playerProgress,
                new Dictionary<TaskType, int>
                {
                    { TaskType.PlaceHeadquarters, 1 }
                },
                cancellationToken);

            await _playerRepository.SaveAsync(playerProgress, cancellationToken);

            return UseCaseResult<PlayerProgress>.Success(playerProgress);
        }
    }
}
