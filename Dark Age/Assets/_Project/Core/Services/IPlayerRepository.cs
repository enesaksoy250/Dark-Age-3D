using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;

namespace DarkAge.Core.Services
{
    public interface IPlayerRepository
    {
        Task<PlayerProgress> LoadAsync(PlayerId playerId, CancellationToken cancellationToken);

        Task SaveAsync(PlayerProgress playerProgress, CancellationToken cancellationToken);
    }
}
