using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;

namespace DarkAge.Infrastructure.Firebase.Auth
{
    public sealed class AdaptiveAuthService : IAuthService
    {
        private readonly IAuthService _primary;
        private readonly IAuthService _fallback;

        public AdaptiveAuthService(IAuthService primary, IAuthService fallback)
        {
            _primary = primary;
            _fallback = fallback;
        }

        public async Task<PlayerId> SignInAnonymouslyAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _primary.SignInAnonymouslyAsync(cancellationToken);
            }
            catch
            {
                return await _fallback.SignInAnonymouslyAsync(cancellationToken);
            }
        }
    }
}
