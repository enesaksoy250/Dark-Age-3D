using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DarkAge.Core.Domain;
using DarkAge.Infrastructure.Firebase.Dtos;

namespace DarkAge.Infrastructure.Firebase.Mapping
{
    public static class FirestoreMapper
    {
        public static PlayerDocumentDto ToPlayerDocument(PlayerProgress playerProgress)
        {
            return new PlayerDocumentDto
            {
                playerId = playerProgress.Profile.PlayerId.Value,
                displayName = playerProgress.Profile.DisplayName,
                createdAtTicks = playerProgress.Profile.CreatedAtUtc.Ticks,
                lastCollectedTicks = playerProgress.LastResourceCollectionUtc.Ticks,
                resources = playerProgress.Resources.AsReadOnlyList()
                    .Select(amount => new ResourceValueDto
                    {
                        resourceType = amount.Type.ToString(),
                        amount = amount.Amount
                    })
                    .ToArray(),
                headquarters = playerProgress.Headquarters == null ? null : ToBaseDocument(playerProgress.Headquarters),
                tasks = playerProgress.Tasks
                    .Select(task => new TaskDocumentDto
                    {
                        taskId = task.TaskId,
                        taskType = task.TaskType.ToString(),
                        currentProgress = task.CurrentProgress,
                        requiredProgress = task.RequiredProgress,
                        rewardGranted = task.RewardGranted
                    })
                    .ToArray()
            };
        }

        public static PlayerProgress FromPlayerDocument(PlayerDocumentDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.playerId))
            {
                return null;
            }

            var resources = new ResourceWallet((dto.resources ?? Array.Empty<ResourceValueDto>())
                .Select(resource => new ResourceAmount(ParseEnum(resource.resourceType, ResourceType.Food), resource.amount)));

            var tasks = (dto.tasks ?? Array.Empty<TaskDocumentDto>())
                .Select(task => new TaskState(
                    task.taskId,
                    ParseEnum(task.taskType, TaskType.PlaceHeadquarters),
                    task.requiredProgress,
                    task.currentProgress,
                    task.rewardGranted));

            return new PlayerProgress(
                new PlayerProfile(new PlayerId(dto.playerId), dto.displayName, new DateTime(dto.createdAtTicks, DateTimeKind.Utc)),
                resources,
                new DateTime(dto.lastCollectedTicks, DateTimeKind.Utc),
                FromBaseDocument(dto.headquarters),
                tasks);
        }

        public static WorldBaseDocumentDto ToWorldBaseDocument(WorldBaseRecord worldBase)
        {
            return new WorldBaseDocumentDto
            {
                ownerId = worldBase.OwnerId.Value,
                ownerName = worldBase.OwnerName,
                headquarters = ToBaseDocument(worldBase.BaseState)
            };
        }

        public static WorldBaseRecord FromWorldBaseDocument(WorldBaseDocumentDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ownerId) || dto.headquarters == null)
            {
                return null;
            }

            return new WorldBaseRecord(new PlayerId(dto.ownerId), dto.ownerName, FromBaseDocument(dto.headquarters));
        }

        public static Dictionary<string, object> ToDictionary(PlayerDocumentDto dto)
        {
            return new Dictionary<string, object>
            {
                { "playerId", dto.playerId },
                { "displayName", dto.displayName },
                { "createdAtTicks", dto.createdAtTicks },
                { "lastCollectedTicks", dto.lastCollectedTicks },
                {
                    "resources",
                    (dto.resources ?? Array.Empty<ResourceValueDto>())
                    .Select(resource => new Dictionary<string, object>
                    {
                        { "resourceType", resource.resourceType },
                        { "amount", resource.amount }
                    })
                    .Cast<object>()
                    .ToList()
                },
                { "headquarters", dto.headquarters == null ? null : ToDictionary(dto.headquarters) },
                {
                    "tasks",
                    (dto.tasks ?? Array.Empty<TaskDocumentDto>())
                    .Select(task => new Dictionary<string, object>
                    {
                        { "taskId", task.taskId },
                        { "taskType", task.taskType },
                        { "currentProgress", task.currentProgress },
                        { "requiredProgress", task.requiredProgress },
                        { "rewardGranted", task.rewardGranted }
                    })
                    .Cast<object>()
                    .ToList()
                }
            };
        }

        public static PlayerDocumentDto FromDictionary(IDictionary dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }

            return new PlayerDocumentDto
            {
                playerId = ReadString(dictionary, "playerId"),
                displayName = ReadString(dictionary, "displayName"),
                createdAtTicks = ReadLong(dictionary, "createdAtTicks"),
                lastCollectedTicks = ReadLong(dictionary, "lastCollectedTicks"),
                resources = ReadList(dictionary, "resources")
                    .Select(item => new ResourceValueDto
                    {
                        resourceType = ReadString(item, "resourceType"),
                        amount = ReadInt(item, "amount")
                    })
                    .ToArray(),
                headquarters = ReadObject(dictionary, "headquarters") == null ? null : FromBaseDictionary(ReadObject(dictionary, "headquarters")),
                tasks = ReadList(dictionary, "tasks")
                    .Select(item => new TaskDocumentDto
                    {
                        taskId = ReadString(item, "taskId"),
                        taskType = ReadString(item, "taskType"),
                        currentProgress = ReadInt(item, "currentProgress"),
                        requiredProgress = ReadInt(item, "requiredProgress"),
                        rewardGranted = ReadBool(item, "rewardGranted")
                    })
                    .ToArray()
            };
        }

        public static Dictionary<string, object> ToDictionary(WorldBaseDocumentDto dto)
        {
            return new Dictionary<string, object>
            {
                { "ownerId", dto.ownerId },
                { "ownerName", dto.ownerName },
                { "headquarters", dto.headquarters == null ? null : ToDictionary(dto.headquarters) }
            };
        }

        public static WorldBaseDocumentDto FromWorldBaseDictionary(IDictionary dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }

            return new WorldBaseDocumentDto
            {
                ownerId = ReadString(dictionary, "ownerId"),
                ownerName = ReadString(dictionary, "ownerName"),
                headquarters = ReadObject(dictionary, "headquarters") == null ? null : FromBaseDictionary(ReadObject(dictionary, "headquarters"))
            };
        }

        private static BaseDocumentDto ToBaseDocument(BaseState baseState)
        {
            return new BaseDocumentDto
            {
                buildingType = baseState.BuildingType.ToString(),
                gridX = baseState.GridPosition.X,
                gridZ = baseState.GridPosition.Z,
                placedAtTicks = baseState.PlacedAtUtc.Ticks
            };
        }

        private static BaseState FromBaseDocument(BaseDocumentDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new BaseState(
                ParseEnum(dto.buildingType, BuildingType.Headquarters),
                new GridPosition(dto.gridX, dto.gridZ),
                new DateTime(dto.placedAtTicks, DateTimeKind.Utc));
        }

        private static Dictionary<string, object> ToDictionary(BaseDocumentDto dto)
        {
            return new Dictionary<string, object>
            {
                { "buildingType", dto.buildingType },
                { "gridX", dto.gridX },
                { "gridZ", dto.gridZ },
                { "placedAtTicks", dto.placedAtTicks }
            };
        }

        private static BaseDocumentDto FromBaseDictionary(IDictionary dictionary)
        {
            return new BaseDocumentDto
            {
                buildingType = ReadString(dictionary, "buildingType"),
                gridX = ReadInt(dictionary, "gridX"),
                gridZ = ReadInt(dictionary, "gridZ"),
                placedAtTicks = ReadLong(dictionary, "placedAtTicks")
            };
        }

        private static TEnum ParseEnum<TEnum>(string value, TEnum fallback)
            where TEnum : struct
        {
            return Enum.TryParse<TEnum>(value, true, out var parsed) ? parsed : fallback;
        }

        private static string ReadString(IDictionary dictionary, string key)
        {
            return dictionary.Contains(key) ? dictionary[key]?.ToString() : string.Empty;
        }

        private static int ReadInt(IDictionary dictionary, string key)
        {
            return dictionary.Contains(key) ? Convert.ToInt32(dictionary[key]) : 0;
        }

        private static long ReadLong(IDictionary dictionary, string key)
        {
            return dictionary.Contains(key) ? Convert.ToInt64(dictionary[key]) : 0L;
        }

        private static bool ReadBool(IDictionary dictionary, string key)
        {
            return dictionary.Contains(key) && Convert.ToBoolean(dictionary[key]);
        }

        private static IDictionary ReadObject(IDictionary dictionary, string key)
        {
            return dictionary.Contains(key) ? dictionary[key] as IDictionary : null;
        }

        private static IEnumerable<IDictionary> ReadList(IDictionary dictionary, string key)
        {
            if (!dictionary.Contains(key) || dictionary[key] == null)
            {
                return Enumerable.Empty<IDictionary>();
            }

            var items = dictionary[key] as IEnumerable;
            if (items == null)
            {
                return Enumerable.Empty<IDictionary>();
            }

            return items.Cast<object>().OfType<IDictionary>();
        }
    }
}
