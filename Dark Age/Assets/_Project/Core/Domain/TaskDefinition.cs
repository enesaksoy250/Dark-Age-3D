using System;
using System.Collections.Generic;

namespace DarkAge.Core.Domain
{
    [Serializable]
    public sealed class TaskDefinition
    {
        public TaskDefinition(string id, TaskType taskType, int requiredProgress, IReadOnlyList<ResourceAmount> rewards)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Task id cannot be empty.", nameof(id));
            }

            if (requiredProgress <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredProgress), "Required progress must be greater than zero.");
            }

            Id = id;
            TaskType = taskType;
            RequiredProgress = requiredProgress;
            Rewards = rewards ?? Array.Empty<ResourceAmount>();
        }

        public string Id { get; }

        public TaskType TaskType { get; }

        public int RequiredProgress { get; }

        public IReadOnlyList<ResourceAmount> Rewards { get; }
    }
}
