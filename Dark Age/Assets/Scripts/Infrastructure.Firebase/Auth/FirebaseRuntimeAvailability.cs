using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DarkAge.Infrastructure.Firebase.Auth
{
    internal sealed class FirebaseRuntimeAvailability
    {
        private bool? _isAvailable;

        public async Task<bool> IsAvailableAsync()
        {
            if (_isAvailable.HasValue)
            {
                return _isAvailable.Value;
            }

            var appType = Type.GetType("Firebase.FirebaseApp, Firebase.App");
            if (appType == null)
            {
                _isAvailable = false;
                return false;
            }

            var checkMethod = appType.GetMethod("CheckAndFixDependenciesAsync", BindingFlags.Public | BindingFlags.Static);
            if (checkMethod == null)
            {
                _isAvailable = false;
                return false;
            }

            var taskInstance = checkMethod.Invoke(null, null);
            await ReflectionTaskUtility.AwaitTaskAsync(taskInstance).ConfigureAwait(false);
            var result = ReflectionTaskUtility.GetTaskResult(taskInstance);
            _isAvailable = string.Equals(result?.ToString(), "Available", StringComparison.OrdinalIgnoreCase);
            return _isAvailable.Value;
        }
    }
}
