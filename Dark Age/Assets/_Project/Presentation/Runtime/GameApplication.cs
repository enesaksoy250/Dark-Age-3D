using System;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;
using DarkAge.Gameplay.Results;
using DarkAge.Gameplay.UseCases;

namespace DarkAge.Presentation.Runtime
{
    public sealed class GameApplication
    {
        private readonly InitializePlayerUseCase _initializePlayerUseCase;
        private readonly GetWorldStateUseCase _getWorldStateUseCase;
        private readonly PlaceHeadquartersUseCase _placeHeadquartersUseCase;
        private readonly CollectProducedResourcesUseCase _collectProducedResourcesUseCase;
        private readonly IGameRulesProvider _gameRulesProvider;

        public GameApplication(
            InitializePlayerUseCase initializePlayerUseCase,
            GetWorldStateUseCase getWorldStateUseCase,
            PlaceHeadquartersUseCase placeHeadquartersUseCase,
            CollectProducedResourcesUseCase collectProducedResourcesUseCase,
            IGameRulesProvider gameRulesProvider)
        {
            _initializePlayerUseCase = initializePlayerUseCase;
            _getWorldStateUseCase = getWorldStateUseCase;
            _placeHeadquartersUseCase = placeHeadquartersUseCase;
            _collectProducedResourcesUseCase = collectProducedResourcesUseCase;
            _gameRulesProvider = gameRulesProvider;
        }

        public event Action<WorldStateSnapshot> StateChanged;
        public event Action<string> ErrorOccurred;

        public PlayerId CurrentPlayerId { get; private set; }

        public WorldStateSnapshot CurrentState { get; private set; }

        public GameRules Rules => _gameRulesProvider.GetRules();

        public async Task<bool> InitializeAsync(CancellationToken cancellationToken)
        {
            var result = await _initializePlayerUseCase.ExecuteAsync(cancellationToken);
            if (!result.IsSuccess)
            {
                ErrorOccurred?.Invoke(result.ErrorMessage);
                return false;
            }

            CurrentPlayerId = result.Value.Profile.PlayerId;
            return await RefreshWorldAsync(cancellationToken);
        }

        public async Task<bool> RefreshWorldAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(CurrentPlayerId.Value))
            {
                return false;
            }

            var result = await _getWorldStateUseCase.ExecuteAsync(CurrentPlayerId, cancellationToken);
            if (!result.IsSuccess)
            {
                ErrorOccurred?.Invoke(result.ErrorMessage);
                return false;
            }

            CurrentState = result.Value;
            StateChanged?.Invoke(CurrentState);
            return true;
        }

        public async Task<bool> PlaceHeadquartersAsync(GridPosition gridPosition, CancellationToken cancellationToken)
        {
            var result = await _placeHeadquartersUseCase.ExecuteAsync(CurrentPlayerId, gridPosition, cancellationToken);
            if (!result.IsSuccess)
            {
                ErrorOccurred?.Invoke(result.ErrorMessage);
                return false;
            }

            return await RefreshWorldAsync(cancellationToken);
        }

        public async Task<bool> CollectResourcesAsync(CancellationToken cancellationToken)
        {
            var result = await _collectProducedResourcesUseCase.ExecuteAsync(CurrentPlayerId, cancellationToken);
            if (!result.IsSuccess)
            {
                ErrorOccurred?.Invoke(result.ErrorMessage);
                return false;
            }

            return await RefreshWorldAsync(cancellationToken);
        }
    }
}
