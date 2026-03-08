using System.Collections.Generic;
using DarkAge.Core.Domain;

namespace DarkAge.Core.Services
{
    public interface ITaskProgressService
    {
        TaskProgressReport Evaluate(
            PlayerProgress playerProgress,
            IReadOnlyList<TaskDefinition> definitions,
            IReadOnlyDictionary<TaskType, int> progressDeltas);
    }
}
