using System;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Core.Services;
using UnityEngine;

namespace DarkAge.Infrastructure.Firebase.Auth
{
    public sealed class LocalAnonymousAuthService : IAuthService
    {
        private const string PlayerIdKey = "DarkAge.LocalPlayerId";

        public Task<PlayerId> SignInAnonymouslyAsync(CancellationToken cancellationToken)
        {
            var playerId = PlayerPrefs.GetString(PlayerIdKey, string.Empty);
            if (string.IsNullOrWhiteSpace(playerId))
            {
                playerId = Guid.NewGuid().ToString("N");
                PlayerPrefs.SetString(PlayerIdKey, playerId);
                PlayerPrefs.Save();
            }

            return Task.FromResult(new PlayerId(playerId));
        }
    }
}
