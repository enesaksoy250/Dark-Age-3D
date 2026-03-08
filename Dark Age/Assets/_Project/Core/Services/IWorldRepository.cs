using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;

namespace DarkAge.Core.Services
{
    public interface IWorldRepository
    {
        Task<IReadOnlyList<WorldBaseRecord>> LoadAllBasesAsync(CancellationToken cancellationToken);

        Task<bool> IsGridOccupiedAsync(GridPosition gridPosition, CancellationToken cancellationToken);

        Task SaveBaseAsync(WorldBaseRecord baseRecord, CancellationToken cancellationToken);
    }
}
