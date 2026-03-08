using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;
using DarkAge.Infrastructure.Firebase.Mapping;

namespace DarkAge.Infrastructure.Firebase.Persistence
{
    public sealed class FirebaseWorldRepository : IWorldRepository
    {
        private const string CollectionName = "worldBases";
        private readonly FirestoreReflectionClient _firestoreClient;

        public FirebaseWorldRepository(FirestoreReflectionClient firestoreClient)
        {
            _firestoreClient = firestoreClient;
        }

        public async Task<IReadOnlyList<WorldBaseRecord>> LoadAllBasesAsync(CancellationToken cancellationToken)
        {
            var collection = await _firestoreClient.GetCollectionAsync(CollectionName, cancellationToken);
            return collection
                .Select(FirestoreMapper.FromWorldBaseDictionary)
                .Select(FirestoreMapper.FromWorldBaseDocument)
                .Where(baseRecord => baseRecord != null)
                .ToArray();
        }

        public async Task<bool> IsGridOccupiedAsync(GridPosition gridPosition, CancellationToken cancellationToken)
        {
            var worldBases = await LoadAllBasesAsync(cancellationToken);
            return worldBases.Any(worldBase => worldBase.BaseState.GridPosition.Equals(gridPosition));
        }

        public Task SaveBaseAsync(WorldBaseRecord baseRecord, CancellationToken cancellationToken)
        {
            var dto = FirestoreMapper.ToWorldBaseDocument(baseRecord);
            return _firestoreClient.SetDocumentAsync(CollectionName, baseRecord.OwnerId.Value, FirestoreMapper.ToDictionary(dto), cancellationToken);
        }
    }
}
