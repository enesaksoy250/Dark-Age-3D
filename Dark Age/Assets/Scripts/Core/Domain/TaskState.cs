using System;

namespace DarkAge.Core.Domain
{
    [Serializable]
    public sealed class TaskState
    {
        public TaskState(string taskId, TaskType taskType, int requiredProgress, int currentProgress, bool rewardGranted)
        {
            if (string.IsNullOrWhiteSpace(taskId))
            {
                throw new ArgumentException("Task id cannot be empty.", nameof(taskId));
            }

            if (requiredProgress <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredProgress), "Required progress must be greater than zero.");
            }

            TaskId = taskId;
            TaskType = taskType;
            RequiredProgress = requiredProgress;
            CurrentProgress = Math.Max(0, currentProgress);
            RewardGranted = rewardGranted;
        }

        public string TaskId { get; }

        public TaskType TaskType { get; }

        public int RequiredProgress { get; }

        public int CurrentProgress { get; private set; }

        public bool RewardGranted { get; private set; }

        public bool IsCompleted => CurrentProgress >= RequiredProgress;

        public void SetProgress(int progress)
        {
            CurrentProgress = Math.Max(0, progress);
        }

        public void AddProgress(int progress)
        {
            if (progress <= 0)
            {
                return;
            }

            CurrentProgress += progress;
        }

        public void MarkRewardGranted()
        {
            RewardGranted = true;
        }
    }
}
