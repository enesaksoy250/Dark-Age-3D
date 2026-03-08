using System;
using DarkAge.Core.Services;

namespace DarkAge.Infrastructure.Firebase.Persistence
{
    public sealed class SystemTimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
