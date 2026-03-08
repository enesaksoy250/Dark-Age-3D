using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;
using DarkAge.Gameplay.Results;

namespace DarkAge.Gameplay.UseCases
{
    public sealed class InitializePlayerUseCase
    {
        private readonly IAuthService _authService;
        private readonly IPlayerRepository _playerRepository;
        private readonly ITimeProvider _timeProvider;
        private readonly IGameRulesProvider _gameRulesProvider;
        private readonly EvaluateTasksUseCase _evaluateTasksUseCase;

        public InitializePlayerUseCase(
            IAuthService authService,
            IPlayerRepository playerRepository,
            ITimeProvider timeProvider,
            IGameRulesProvider gameRulesProvider,
            EvaluateTasksUseCase evaluateTasksUseCase)
        {
            _authService = authService;
            _playerRepository = playerRepository;
            _timeProvider = timeProvider;
            _gameRulesProvider = gameRulesProvider;
            _evaluateTasksUseCase = evaluateTasksUseCase;
        }

        public async Task<UseCaseResult<PlayerProgress>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var playerId = await _authService.SignInAnonymouslyAsync(cancellationToken);
            var playerProgress = await _playerRepository.LoadAsync(playerId, cancellationToken);
            if (playerProgress == null)
            {
                var rules = _gameRulesProvider.GetRules();
                var displayName = $"Warden-{playerId.Value.Substring(Math.Max(0, playerId.Value.Length - 4))}";
                playerProgress = new PlayerProgress(
                    new PlayerProfile(playerId, displayName, _timeProvider.UtcNow),
                    rules.StartingResources,
                    _timeProvider.UtcNow,
                    null,
                    Array.Empty<TaskState>());

                await _playerRepository.SaveAsync(playerProgress, cancellationToken);
            }

            await _evaluateTasksUseCase.ExecuteAsync(
                playerProgress,
                new Dictionary<TaskType, int>(),
                cancellationToken);

            return UseCaseResult<PlayerProgress>.Success(playerProgress);
        }
    }
}
