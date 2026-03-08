using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;

namespace DarkAge.Gameplay.UseCases
{
    public sealed class EvaluateTasksUseCase
    {
        private readonly IGameRulesProvider _gameRulesProvider;
        private readonly ITaskProgressService _taskProgressService;
        private readonly IPlayerRepository _playerRepository;

        public EvaluateTasksUseCase(
            IGameRulesProvider gameRulesProvider,
            ITaskProgressService taskProgressService,
            IPlayerRepository playerRepository)
        {
            _gameRulesProvider = gameRulesProvider;
            _taskProgressService = taskProgressService;
            _playerRepository = playerRepository;
        }

        public async Task<TaskProgressReport> ExecuteAsync(
            PlayerProgress playerProgress,
            IReadOnlyDictionary<TaskType, int> progressDeltas,
            CancellationToken cancellationToken)
        {
            var rules = _gameRulesProvider.GetRules();
            var report = _taskProgressService.Evaluate(playerProgress, rules.TaskDefinitions, progressDeltas);
            if (report.Changed)
            {
                await _playerRepository.SaveAsync(playerProgress, cancellationToken);
            }

            return report;
        }
    }
}
