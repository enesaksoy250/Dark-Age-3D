using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;

namespace DarkAge.Core.Services
{
    public interface IAuthService
    {
        Task<PlayerId> SignInAnonymouslyAsync(CancellationToken cancellationToken);
    }
}
