using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;

namespace DarkAge.Infrastructure.Firebase.Persistence
{
    public sealed class AdaptiveWorldRepository : IWorldRepository
    {
        private readonly IWorldRepository _primary;
        private readonly IWorldRepository _fallback;

        public AdaptiveWorldRepository(IWorldRepository primary, IWorldRepository fallback)
        {
            _primary = primary;
            _fallback = fallback;
        }

        public async Task<IReadOnlyList<WorldBaseRecord>> LoadAllBasesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _primary.LoadAllBasesAsync(cancellationToken);
            }
            catch
            {
                return await _fallback.LoadAllBasesAsync(cancellationToken);
            }
        }

        public async Task<bool> IsGridOccupiedAsync(GridPosition gridPosition, CancellationToken cancellationToken)
        {
            try
            {
                return await _primary.IsGridOccupiedAsync(gridPosition, cancellationToken);
            }
            catch
            {
                return await _fallback.IsGridOccupiedAsync(gridPosition, cancellationToken);
            }
        }

        public async Task SaveBaseAsync(WorldBaseRecord baseRecord, CancellationToken cancellationToken)
        {
            try
            {
                await _primary.SaveBaseAsync(baseRecord, cancellationToken);
            }
            catch
            {
                await _fallback.SaveBaseAsync(baseRecord, cancellationToken);
            }
        }
    }
}
