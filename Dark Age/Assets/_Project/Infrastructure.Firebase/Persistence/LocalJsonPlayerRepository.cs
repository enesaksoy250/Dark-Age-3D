using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;
using DarkAge.Infrastructure.Firebase.Mapping;
using UnityEngine;

namespace DarkAge.Infrastructure.Firebase.Persistence
{
    public sealed class LocalJsonPlayerRepository : IPlayerRepository
    {
        public Task<PlayerProgress> LoadAsync(PlayerId playerId, CancellationToken cancellationToken)
        {
            var path = LocalPersistencePaths.GetPlayerFilePath(playerId.Value);
            if (!File.Exists(path))
            {
                return Task.FromResult<PlayerProgress>(null);
            }

            var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                return Task.FromResult<PlayerProgress>(null);
            }

            var dto = JsonUtility.FromJson<Dtos.PlayerDocumentDto>(json);
            return Task.FromResult(FirestoreMapper.FromPlayerDocument(dto));
        }

        public Task SaveAsync(PlayerProgress playerProgress, CancellationToken cancellationToken)
        {
            var path = LocalPersistencePaths.GetPlayerFilePath(playerProgress.Profile.PlayerId.Value);
            var dto = FirestoreMapper.ToPlayerDocument(playerProgress);
            var json = JsonUtility.ToJson(dto, true);
            File.WriteAllText(path, json);
            return Task.CompletedTask;
        }
    }
}
