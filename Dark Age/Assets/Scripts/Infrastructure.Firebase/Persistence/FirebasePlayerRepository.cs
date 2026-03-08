using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;
using DarkAge.Infrastructure.Firebase.Mapping;

namespace DarkAge.Infrastructure.Firebase.Persistence
{
    public sealed class FirebasePlayerRepository : IPlayerRepository
    {
        private const string CollectionName = "players";
        private readonly FirestoreReflectionClient _firestoreClient;

        public FirebasePlayerRepository(FirestoreReflectionClient firestoreClient)
        {
            _firestoreClient = firestoreClient;
        }

        public async Task<PlayerProgress> LoadAsync(PlayerId playerId, CancellationToken cancellationToken)
        {
            var dictionary = await _firestoreClient.GetDocumentAsync(CollectionName, playerId.Value, cancellationToken);
            var dto = FirestoreMapper.FromDictionary(dictionary);
            return FirestoreMapper.FromPlayerDocument(dto);
        }

        public Task SaveAsync(PlayerProgress playerProgress, CancellationToken cancellationToken)
        {
            var dto = FirestoreMapper.ToPlayerDocument(playerProgress);
            return _firestoreClient.SetDocumentAsync(CollectionName, playerProgress.Profile.PlayerId.Value, FirestoreMapper.ToDictionary(dto), cancellationToken);
        }
    }
}
