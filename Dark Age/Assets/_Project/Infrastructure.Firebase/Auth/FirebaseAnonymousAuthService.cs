using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;

namespace DarkAge.Infrastructure.Firebase.Auth
{
    public sealed class FirebaseAnonymousAuthService : IAuthService
    {
        private readonly FirebaseRuntimeAvailability _runtimeAvailability;

        public FirebaseAnonymousAuthService(FirebaseRuntimeAvailability runtimeAvailability)
        {
            _runtimeAvailability = runtimeAvailability;
        }

        public async Task<PlayerId> SignInAnonymouslyAsync(CancellationToken cancellationToken)
        {
            if (!await _runtimeAvailability.IsAvailableAsync().ConfigureAwait(false))
            {
                throw new InvalidOperationException("Firebase runtime is not available.");
            }

            var authType = Type.GetType("Firebase.Auth.FirebaseAuth, Firebase.Auth");
            if (authType == null)
            {
                throw new InvalidOperationException("Firebase.Auth assembly could not be located.");
            }

            var authInstance = authType.GetProperty("DefaultInstance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
            if (authInstance == null)
            {
                throw new InvalidOperationException("FirebaseAuth.DefaultInstance is unavailable.");
            }

            var signInMethod = authType.GetMethod("SignInAnonymouslyAsync", Type.EmptyTypes);
            var signInTask = signInMethod?.Invoke(authInstance, null);
            await ReflectionTaskUtility.AwaitTaskAsync(signInTask).ConfigureAwait(false);

            var currentUser = authInstance.GetType().GetProperty("CurrentUser", BindingFlags.Public | BindingFlags.Instance)?.GetValue(authInstance);
            var userId = currentUser?.GetType().GetProperty("UserId", BindingFlags.Public | BindingFlags.Instance)?.GetValue(currentUser)?.ToString();
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new InvalidOperationException("Firebase anonymous sign-in did not produce a user id.");
            }

            return new PlayerId(userId);
        }
    }
}
