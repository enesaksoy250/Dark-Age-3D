using UnityEngine;

namespace DarkAge.Presentation.Runtime
{
    public static class DarkAgeRuntimeEntrypoint
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (Object.FindAnyObjectByType<DarkAgeWorldBootstrapper>() != null)
            {
                return;
            }

            var bootstrapObject = new GameObject("DarkAgeBootstrap");
            bootstrapObject.AddComponent<DarkAgeWorldBootstrapper>();
        }
    }
}
