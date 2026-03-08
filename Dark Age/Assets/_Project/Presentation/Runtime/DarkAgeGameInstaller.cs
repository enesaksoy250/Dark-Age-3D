using DarkAge.Core.Services;
using DarkAge.Gameplay.Services;
using DarkAge.Gameplay.UseCases;
using DarkAge.Infrastructure.Firebase.Auth;
using DarkAge.Infrastructure.Firebase.Persistence;
using DarkAge.Presentation.Config;

namespace DarkAge.Presentation.Runtime
{
    public static class DarkAgeGameInstaller
    {
        public static GameApplication CreateApplication(GameBalanceConfig gameBalanceConfig, WorldConfig worldConfig)
        {
            var rulesProvider = new ScriptableGameRulesProvider(gameBalanceConfig, worldConfig);

            var runtimeAvailability = new FirebaseRuntimeAvailability();
            var authService = new AdaptiveAuthService(
                new FirebaseAnonymousAuthService(runtimeAvailability),
                new LocalAnonymousAuthService());

            var firestoreClient = new FirestoreReflectionClient(runtimeAvailability);
            var playerRepository = new AdaptivePlayerRepository(
                new FirebasePlayerRepository(firestoreClient),
                new LocalJsonPlayerRepository());

            var worldRepository = new AdaptiveWorldRepository(
                new FirebaseWorldRepository(firestoreClient),
                new LocalJsonWorldRepository());

            ITimeProvider timeProvider = new SystemTimeProvider();
            IResourceTickService resourceTickService = new DefaultResourceTickService();
            ITaskProgressService taskProgressService = new DefaultTaskProgressService();

            var evaluateTasksUseCase = new EvaluateTasksUseCase(rulesProvider, taskProgressService, playerRepository);
            var initializePlayerUseCase = new InitializePlayerUseCase(authService, playerRepository, timeProvider, rulesProvider, evaluateTasksUseCase);
            var placeHeadquartersUseCase = new PlaceHeadquartersUseCase(playerRepository, worldRepository, rulesProvider, timeProvider, evaluateTasksUseCase);
            var collectProducedResourcesUseCase = new CollectProducedResourcesUseCase(playerRepository, rulesProvider, resourceTickService, timeProvider, evaluateTasksUseCase);
            var getWorldStateUseCase = new GetWorldStateUseCase(playerRepository, worldRepository);

            return new GameApplication(
                initializePlayerUseCase,
                getWorldStateUseCase,
                placeHeadquartersUseCase,
                collectProducedResourcesUseCase,
                rulesProvider);
        }
    }
}
