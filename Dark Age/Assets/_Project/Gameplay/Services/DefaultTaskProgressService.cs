using System.Collections.Generic;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;

namespace DarkAge.Gameplay.Services
{
    public sealed class DefaultTaskProgressService : ITaskProgressService
    {
        public TaskProgressReport Evaluate(
            PlayerProgress playerProgress,
            IReadOnlyList<TaskDefinition> definitions,
            IReadOnlyDictionary<TaskType, int> progressDeltas)
        {
            var changed = false;
            var grantedRewards = ResourceWallet.Empty();

            foreach (var definition in definitions)
            {
                var task = playerProgress.GetOrAddTask(definition);
                var previousProgress = task.CurrentProgress;
                var previousRewardState = task.RewardGranted;

                switch (definition.TaskType)
                {
                    case TaskType.PlaceHeadquarters:
                        if (playerProgress.HasHeadquarters)
                        {
                            task.SetProgress(definition.RequiredProgress);
                        }
                        break;
                    case TaskType.CollectFood:
                        task.AddProgress(GetProgressDelta(progressDeltas, TaskType.CollectFood));
                        break;
                    case TaskType.CollectGold:
                        task.AddProgress(GetProgressDelta(progressDeltas, TaskType.CollectGold));
                        break;
                    case TaskType.CollectAnyResources:
                        task.AddProgress(GetProgressDelta(progressDeltas, TaskType.CollectAnyResources));
                        break;
                }

                if (task.IsCompleted && !task.RewardGranted)
                {
                    foreach (var reward in definition.Rewards)
                    {
                        grantedRewards.Add(reward.Type, reward.Amount);
                    }

                    task.MarkRewardGranted();
                }

                if (task.CurrentProgress != previousProgress || task.RewardGranted != previousRewardState)
                {
                    changed = true;
                }
            }

            if (grantedRewards.TotalAmount > 0)
            {
                playerProgress.Resources.Add(grantedRewards);
                changed = true;
            }

            return new TaskProgressReport(grantedRewards, changed);
        }

        private static int GetProgressDelta(IReadOnlyDictionary<TaskType, int> progressDeltas, TaskType taskType)
        {
            return progressDeltas != null && progressDeltas.TryGetValue(taskType, out var value) ? value : 0;
        }
    }
}
