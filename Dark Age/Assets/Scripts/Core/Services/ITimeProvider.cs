using System;

namespace DarkAge.Core.Services
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }
}
