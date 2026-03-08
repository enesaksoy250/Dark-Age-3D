using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;
using DarkAge.Gameplay.Results;

namespace DarkAge.Gameplay.UseCases
{
    public sealed class GetWorldStateUseCase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IWorldRepository _worldRepository;

        public GetWorldStateUseCase(
            IPlayerRepository playerRepository,
            IWorldRepository worldRepository)
        {
            _playerRepository = playerRepository;
            _worldRepository = worldRepository;
        }

        public async Task<UseCaseResult<WorldStateSnapshot>> ExecuteAsync(PlayerId playerId, CancellationToken cancellationToken)
        {
            var playerProgress = await _playerRepository.LoadAsync(playerId, cancellationToken);
            if (playerProgress == null)
            {
                return UseCaseResult<WorldStateSnapshot>.Failure("Player progress could not be loaded.");
            }

            var worldBases = await _worldRepository.LoadAllBasesAsync(cancellationToken);
            return UseCaseResult<WorldStateSnapshot>.Success(new WorldStateSnapshot(playerProgress, worldBases));
        }
    }
}
