using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;

namespace DarkAge.Infrastructure.Firebase.Persistence
{
    public sealed class AdaptivePlayerRepository : IPlayerRepository
    {
        private readonly IPlayerRepository _primary;
        private readonly IPlayerRepository _fallback;

        public AdaptivePlayerRepository(IPlayerRepository primary, IPlayerRepository fallback)
        {
            _primary = primary;
            _fallback = fallback;
        }

        public async Task<PlayerProgress> LoadAsync(PlayerId playerId, CancellationToken cancellationToken)
        {
            try
            {
                return await _primary.LoadAsync(playerId, cancellationToken);
            }
            catch
            {
                return await _fallback.LoadAsync(playerId, cancellationToken);
            }
        }

        public async Task SaveAsync(PlayerProgress playerProgress, CancellationToken cancellationToken)
        {
            try
            {
                await _primary.SaveAsync(playerProgress, cancellationToken);
            }
            catch
            {
                await _fallback.SaveAsync(playerProgress, cancellationToken);
            }
        }
    }
}
