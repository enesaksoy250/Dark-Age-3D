using DarkAge.Presentation.Config;
using DarkAge.Presentation.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkAge.Presentation.Runtime
{
    public sealed class DarkAgeWorldBootstrapper : MonoBehaviour
    {
        private GameApplication _application;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            EnsureSceneController();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void EnsureSceneController()
        {
            if (Object.FindAnyObjectByType<WorldSceneController>() != null)
            {
                return;
            }

            if (_application == null)
            {
                var gameBalanceConfig = Resources.Load<GameBalanceConfig>("DarkAge/GameBalanceConfig") ?? GameBalanceConfig.CreateDefault();
                var worldConfig = Resources.Load<WorldConfig>("DarkAge/WorldConfig") ?? WorldConfig.CreateDefault();
                _application = DarkAgeGameInstaller.CreateApplication(gameBalanceConfig, worldConfig);
            }

            var controllerObject = new GameObject("DarkAgeWorldSceneController");
            var controller = controllerObject.AddComponent<WorldSceneController>();
            controller.Initialize(_application);
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EnsureSceneController();
        }
    }
}
