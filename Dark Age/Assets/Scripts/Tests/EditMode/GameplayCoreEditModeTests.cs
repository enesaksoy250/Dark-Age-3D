using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;
using DarkAge.Gameplay.Services;
using DarkAge.Gameplay.UseCases;
using DarkAge.Infrastructure.Firebase.Mapping;
using NUnit.Framework;
using UnityEngine;

namespace DarkAge.Tests.EditMode
{
    public sealed class GameplayCoreEditModeTests
    {
        [SetUp]
        public void SetUp()
        {
            var root = Path.Combine(Application.persistentDataPath, "DarkAge");
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
        }

        [Test]
        public async Task PlayerCannotPlaceHeadquartersTwice()
        {
            var context = CreateContext();
            var player = CreatePlayer();
            await context.PlayerRepository.SaveAsync(player, CancellationToken.None);

            var firstPlacement = await context.PlaceHeadquartersUseCase.ExecuteAsync(player.Profile.PlayerId, new GridPosition(0, 0), CancellationToken.None);
            var secondPlacement = await context.PlaceHeadquartersUseCase.ExecuteAsync(player.Profile.PlayerId, new GridPosition(1, 1), CancellationToken.None);

            Assert.That(firstPlacement.IsSuccess, Is.True);
            Assert.That(secondPlacement.IsSuccess, Is.False);
            Assert.That(secondPlacement.ErrorMessage, Does.Contain("already placed"));
        }

        [Test]
        public async Task HeadquartersPlacementRejectsOutOfBoundsGrid()
        {
            var context = CreateContext();
            var player = CreatePlayer();
            await context.PlayerRepository.SaveAsync(player, CancellationToken.None);

            var result = await context.PlaceHeadquartersUseCase.ExecuteAsync(player.Profile.PlayerId, new GridPosition(99, 99), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("outside world bounds"));
        }

        [Test]
        public void ResourceProductionUsesElapsedWholeMinutes()
        {
            var service = new DefaultResourceTickService();
            var rules = new[]
            {
                new ResourceProductionRule(ResourceType.Food, 4),
                new ResourceProductionRule(ResourceType.Gold, 2)
            };

            var result = service.CalculateProducedResources(
                new DateTime(2026, 3, 8, 10, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 3, 8, 10, 5, 45, DateTimeKind.Utc),
                rules);

            Assert.That(result.Get(ResourceType.Food), Is.EqualTo(20));
            Assert.That(result.Get(ResourceType.Gold), Is.EqualTo(10));
        }

        [Test]
        public void TaskRewardsAreGrantedOnlyOnce()
        {
            var rules = CreateRules();
            var player = CreatePlayer();
            player.PlaceHeadquarters(new BaseState(BuildingType.Headquarters, new GridPosition(0, 0), DateTime.UtcNow));

            var service = new DefaultTaskProgressService();
            service.Evaluate(player, rules.TaskDefinitions, new Dictionary<TaskType, int> { { TaskType.PlaceHeadquarters, 1 } });
            var firstFood = player.Resources.Get(ResourceType.Food);

            service.Evaluate(player, rules.TaskDefinitions, new Dictionary<TaskType, int> { { TaskType.PlaceHeadquarters, 1 } });
            var secondFood = player.Resources.Get(ResourceType.Food);

            Assert.That(firstFood, Is.EqualTo(secondFood));
        }

        [Test]
        public void FirestoreMapperRoundTripPreservesPlayerProgress()
        {
            var player = CreatePlayer();
            player.PlaceHeadquarters(new BaseState(BuildingType.Headquarters, new GridPosition(2, 3), new DateTime(2026, 3, 8, 12, 0, 0, DateTimeKind.Utc)));
            player.GetOrAddTask(new TaskDefinition("task_place_hq", TaskType.PlaceHeadquarters, 1, Array.Empty<ResourceAmount>())).SetProgress(1);

            var dto = FirestoreMapper.ToPlayerDocument(player);
            var dictionary = FirestoreMapper.ToDictionary(dto);
            var restored = FirestoreMapper.FromPlayerDocument(FirestoreMapper.FromDictionary(dictionary));

            Assert.That(restored.Profile.PlayerId.Value, Is.EqualTo(player.Profile.PlayerId.Value));
            Assert.That(restored.Headquarters.GridPosition, Is.EqualTo(player.Headquarters.GridPosition));
            Assert.That(restored.Tasks.Count, Is.EqualTo(player.Tasks.Count));
        }

        private static TestContext CreateContext()
        {
            var rulesProvider = new FakeRulesProvider(CreateRules());
            var playerRepository = new InMemoryPlayerRepository();
            var worldRepository = new InMemoryWorldRepository();
            var timeProvider = new FixedTimeProvider(new DateTime(2026, 3, 8, 10, 0, 0, DateTimeKind.Utc));
            var evaluateTasksUseCase = new EvaluateTasksUseCase(rulesProvider, new DefaultTaskProgressService(), playerRepository);

            return new TestContext(
                playerRepository,
                new PlaceHeadquartersUseCase(playerRepository, worldRepository, rulesProvider, timeProvider, evaluateTasksUseCase));
        }

        private static PlayerProgress CreatePlayer()
        {
            return new PlayerProgress(
                new PlayerProfile(new PlayerId("test-player"), "Tester", new DateTime(2026, 3, 8, 9, 0, 0, DateTimeKind.Utc)),
                CreateRules().StartingResources,
                new DateTime(2026, 3, 8, 9, 0, 0, DateTimeKind.Utc),
                null,
                Array.Empty<TaskState>());
        }

        private static GameRules CreateRules()
        {
            return new GameRules(
                new ResourceWallet(new[]
                {
                    new ResourceAmount(ResourceType.Food, 100),
                    new ResourceAmount(ResourceType.Gold, 50)
                }),
                new[]
                {
                    new ResourceProductionRule(ResourceType.Food, 2)
                },
                new[]
                {
                    new TaskDefinition("task_place_hq", TaskType.PlaceHeadquarters, 1, new[]
                    {
                        new ResourceAmount(ResourceType.Food, 25)
                    })
                },
                new WorldBounds(-5, 5, -5, 5),
                2f);
        }

        private sealed class TestContext
        {
            public TestContext(InMemoryPlayerRepository playerRepository, PlaceHeadquartersUseCase placeHeadquartersUseCase)
            {
                PlayerRepository = playerRepository;
                PlaceHeadquartersUseCase = placeHeadquartersUseCase;
            }

            public InMemoryPlayerRepository PlayerRepository { get; }
            public PlaceHeadquartersUseCase PlaceHeadquartersUseCase { get; }
        }

        private sealed class FakeRulesProvider : IGameRulesProvider
        {
            private readonly GameRules _rules;

            public FakeRulesProvider(GameRules rules)
            {
                _rules = rules;
            }

            public GameRules GetRules()
            {
                return _rules;
            }
        }

        private sealed class InMemoryPlayerRepository : IPlayerRepository
        {
            private readonly Dictionary<string, PlayerProgress> _storage = new Dictionary<string, PlayerProgress>();

            public Task<PlayerProgress> LoadAsync(PlayerId playerId, CancellationToken cancellationToken)
            {
                _storage.TryGetValue(playerId.Value, out var player);
                return Task.FromResult(player);
            }

            public Task SaveAsync(PlayerProgress playerProgress, CancellationToken cancellationToken)
            {
                _storage[playerProgress.Profile.PlayerId.Value] = playerProgress;
                return Task.CompletedTask;
            }
        }

        private sealed class InMemoryWorldRepository : IWorldRepository
        {
            private readonly List<WorldBaseRecord> _bases = new List<WorldBaseRecord>();

            public Task<IReadOnlyList<WorldBaseRecord>> LoadAllBasesAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult<IReadOnlyList<WorldBaseRecord>>(_bases);
            }

            public Task<bool> IsGridOccupiedAsync(GridPosition gridPosition, CancellationToken cancellationToken)
            {
                return Task.FromResult(_bases.Exists(item => item.BaseState.GridPosition.Equals(gridPosition)));
            }

            public Task SaveBaseAsync(WorldBaseRecord baseRecord, CancellationToken cancellationToken)
            {
                _bases.RemoveAll(item => item.OwnerId.Equals(baseRecord.OwnerId));
                _bases.Add(baseRecord);
                return Task.CompletedTask;
            }
        }

        private sealed class FixedTimeProvider : ITimeProvider
        {
            public FixedTimeProvider(DateTime utcNow)
            {
                UtcNow = utcNow;
            }

            public DateTime UtcNow { get; }
        }
    }
}
