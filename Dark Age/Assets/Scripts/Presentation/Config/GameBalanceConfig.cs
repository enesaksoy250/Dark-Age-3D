using System;
using System.Linq;
using DarkAge.Core.Domain;
using UnityEngine;

namespace DarkAge.Presentation.Config
{
    [CreateAssetMenu(fileName = "GameBalanceConfig", menuName = "Dark Age/Config/Game Balance")]
    public sealed class GameBalanceConfig : ScriptableObject
    {
        [SerializeField] private StartingResourceEntry[] startingResources =
        {
            new StartingResourceEntry(ResourceType.Food, 200),
            new StartingResourceEntry(ResourceType.Gold, 100),
            new StartingResourceEntry(ResourceType.Iron, 75),
            new StartingResourceEntry(ResourceType.Money, 50),
            new StartingResourceEntry(ResourceType.Petroleum, 25),
            new StartingResourceEntry(ResourceType.Power, 10)
        };

        [SerializeField] private ProductionRuleEntry[] productionRules =
        {
            new ProductionRuleEntry(ResourceType.Food, 6),
            new ProductionRuleEntry(ResourceType.Gold, 3),
            new ProductionRuleEntry(ResourceType.Iron, 2),
            new ProductionRuleEntry(ResourceType.Power, 1)
        };

        [SerializeField] private TaskDefinitionEntry[] taskDefinitions =
        {
            new TaskDefinitionEntry("task_place_hq", TaskType.PlaceHeadquarters, 1, new[]
            {
                new StartingResourceEntry(ResourceType.Food, 100),
                new StartingResourceEntry(ResourceType.Gold, 50)
            }),
            new TaskDefinitionEntry("task_collect_resources", TaskType.CollectAnyResources, 100, new[]
            {
                new StartingResourceEntry(ResourceType.Iron, 25),
                new StartingResourceEntry(ResourceType.Money, 25)
            })
        };

        public ResourceWallet CreateStartingWallet()
        {
            return new ResourceWallet(startingResources.Select(entry => new ResourceAmount(entry.ResourceType, entry.Amount)));
        }

        public ResourceProductionRule[] CreateProductionRules()
        {
            return productionRules.Select(entry => new ResourceProductionRule(entry.ResourceType, entry.AmountPerMinute)).ToArray();
        }

        public TaskDefinition[] CreateTaskDefinitions()
        {
            return taskDefinitions.Select(entry =>
                new TaskDefinition(
                    entry.Id,
                    entry.TaskType,
                    entry.RequiredProgress,
                    entry.Rewards.Select(reward => new ResourceAmount(reward.ResourceType, reward.Amount)).ToArray()))
                .ToArray();
        }

        public static GameBalanceConfig CreateDefault()
        {
            return CreateInstance<GameBalanceConfig>();
        }

        [Serializable]
        public struct StartingResourceEntry
        {
            public StartingResourceEntry(ResourceType resourceType, int amount)
            {
                ResourceType = resourceType;
                Amount = amount;
            }

            public ResourceType ResourceType;
            public int Amount;
        }

        [Serializable]
        public struct ProductionRuleEntry
        {
            public ProductionRuleEntry(ResourceType resourceType, int amountPerMinute)
            {
                ResourceType = resourceType;
                AmountPerMinute = amountPerMinute;
            }

            public ResourceType ResourceType;
            public int AmountPerMinute;
        }

        [Serializable]
        public struct TaskDefinitionEntry
        {
            public TaskDefinitionEntry(string id, TaskType taskType, int requiredProgress, StartingResourceEntry[] rewards)
            {
                Id = id;
                TaskType = taskType;
                RequiredProgress = requiredProgress;
                Rewards = rewards;
            }

            public string Id;
            public TaskType TaskType;
            public int RequiredProgress;
            public StartingResourceEntry[] Rewards;
        }
    }
}
