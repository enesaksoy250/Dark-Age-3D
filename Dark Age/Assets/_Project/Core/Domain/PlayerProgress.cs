using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkAge.Core.Domain
{
    [Serializable]
    public sealed class PlayerProgress
    {
        private readonly List<TaskState> _tasks;

        public PlayerProgress(
            PlayerProfile profile,
            ResourceWallet resources,
            DateTime lastResourceCollectionUtc,
            BaseState headquarters,
            IEnumerable<TaskState> tasks)
        {
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            Resources = resources?.Clone() ?? throw new ArgumentNullException(nameof(resources));
            LastResourceCollectionUtc = lastResourceCollectionUtc;
            Headquarters = headquarters;
            _tasks = tasks?.Select(task => new TaskState(task.TaskId, task.TaskType, task.RequiredProgress, task.CurrentProgress, task.RewardGranted)).ToList()
                ?? new List<TaskState>();
        }

        public PlayerProfile Profile { get; }

        public ResourceWallet Resources { get; }

        public DateTime LastResourceCollectionUtc { get; private set; }

        public BaseState Headquarters { get; private set; }

        public IReadOnlyList<TaskState> Tasks => _tasks;

        public bool HasHeadquarters => Headquarters != null;

        public void PlaceHeadquarters(BaseState headquarters)
        {
            Headquarters = headquarters ?? throw new ArgumentNullException(nameof(headquarters));
        }

        public void ApplyCollectedResources(ResourceWallet collectedResources, DateTime collectedAtUtc)
        {
            if (collectedResources == null)
            {
                throw new ArgumentNullException(nameof(collectedResources));
            }

            Resources.Add(collectedResources);
            LastResourceCollectionUtc = collectedAtUtc;
        }

        public TaskState GetTask(string taskId)
        {
            return _tasks.FirstOrDefault(task => task.TaskId == taskId);
        }

        public TaskState GetOrAddTask(TaskDefinition definition)
        {
            var existingTask = GetTask(definition.Id);
            if (existingTask != null)
            {
                return existingTask;
            }

            var createdTask = new TaskState(definition.Id, definition.TaskType, definition.RequiredProgress, 0, false);
            _tasks.Add(createdTask);
            return createdTask;
        }
    }
}
