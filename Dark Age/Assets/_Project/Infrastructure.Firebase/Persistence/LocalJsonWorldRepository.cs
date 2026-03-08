using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;
using DarkAge.Infrastructure.Firebase.Dtos;
using DarkAge.Infrastructure.Firebase.Mapping;
using UnityEngine;

namespace DarkAge.Infrastructure.Firebase.Persistence
{
    public sealed class LocalJsonWorldRepository : IWorldRepository
    {
        public Task<IReadOnlyList<WorldBaseRecord>> LoadAllBasesAsync(CancellationToken cancellationToken)
        {
            var collection = LoadCollection();
            var bases = (collection.bases ?? new WorldBaseDocumentDto[0])
                .Select(FirestoreMapper.FromWorldBaseDocument)
                .Where(baseRecord => baseRecord != null)
                .ToArray();
            return Task.FromResult<IReadOnlyList<WorldBaseRecord>>(bases);
        }

        public async Task<bool> IsGridOccupiedAsync(GridPosition gridPosition, CancellationToken cancellationToken)
        {
            var bases = await LoadAllBasesAsync(cancellationToken);
            return bases.Any(existingBase => existingBase.BaseState.GridPosition.Equals(gridPosition));
        }

        public Task SaveBaseAsync(WorldBaseRecord baseRecord, CancellationToken cancellationToken)
        {
            var collection = LoadCollection();
            var entries = (collection.bases ?? new WorldBaseDocumentDto[0]).ToList();
            entries.RemoveAll(entry => entry.ownerId == baseRecord.OwnerId.Value);
            entries.Add(FirestoreMapper.ToWorldBaseDocument(baseRecord));
            collection.bases = entries.ToArray();
            SaveCollection(collection);
            return Task.CompletedTask;
        }

        private static WorldBaseCollectionDto LoadCollection()
        {
            var path = LocalPersistencePaths.WorldFilePath;
            if (!File.Exists(path))
            {
                return new WorldBaseCollectionDto
                {
                    bases = new WorldBaseDocumentDto[0]
                };
            }

            var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new WorldBaseCollectionDto
                {
                    bases = new WorldBaseDocumentDto[0]
                };
            }

            return JsonUtility.FromJson<WorldBaseCollectionDto>(json) ?? new WorldBaseCollectionDto
            {
                bases = new WorldBaseDocumentDto[0]
            };
        }

        private static void SaveCollection(WorldBaseCollectionDto collection)
        {
            var path = LocalPersistencePaths.WorldFilePath;
            var json = JsonUtility.ToJson(collection, true);
            File.WriteAllText(path, json);
        }
    }
}
