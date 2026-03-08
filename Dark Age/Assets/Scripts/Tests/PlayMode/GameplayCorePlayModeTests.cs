using System.Collections;
using DarkAge.Core.Domain;
using DarkAge.Gameplay.Results;
using DarkAge.Presentation.Config;
using DarkAge.Presentation.Runtime;
using DarkAge.Presentation.UI;
using DarkAge.Presentation.World;
using NUnit.Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace DarkAge.Tests.PlayMode
{
    public sealed class GameplayCorePlayModeTests
    {
        [UnityTest]
        public IEnumerator BootstrapCreatesWorldControllerAndCanvas()
        {
            var bootstrapObject = new GameObject("Bootstrap");
            bootstrapObject.AddComponent<DarkAgeWorldBootstrapper>();

            yield return null;
            yield return null;

            Assert.That(Object.FindAnyObjectByType<WorldSceneController>(), Is.Not.Null);
            Assert.That(Object.FindAnyObjectByType<Canvas>(), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator GameApplicationInitializesPlayerState()
        {
            var application = DarkAgeGameInstaller.CreateApplication(GameBalanceConfig.CreateDefault(), WorldConfig.CreateDefault());

            yield return Await(application.InitializeAsync(default));

            Assert.That(application.CurrentState, Is.Not.Null);
            Assert.That(application.CurrentState.PlayerProgress.Profile.PlayerId.Value, Is.Not.Empty);
        }

        [UnityTest]
        public IEnumerator ResourceHudRendersResourceValues()
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas));
            var presenter = canvasObject.AddComponent<ResourceHudPresenter>();
            presenter.Build(canvasObject.transform);

            var snapshot = new WorldStateSnapshot(
                new PlayerProgress(
                    new PlayerProfile(new PlayerId("hud-player"), "HUD", System.DateTime.UtcNow),
                    new ResourceWallet(new[]
                    {
                        new ResourceAmount(ResourceType.Food, 123),
                        new ResourceAmount(ResourceType.Gold, 45)
                    }),
                    System.DateTime.UtcNow,
                    null,
                    new TaskState[0]),
                new WorldBaseRecord[0]);

            presenter.Render(snapshot, null);
            yield return null;

            var textValues = presenter.GetComponentsInChildren<UnityEngine.UI.Text>().Select(text => text.text).ToArray();
            Assert.That(textValues, Has.Some.Contains("Food: 123"));
        }

        private static IEnumerator Await(System.Threading.Tasks.Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
    }
}
