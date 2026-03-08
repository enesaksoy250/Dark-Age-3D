using DarkAge.Core.Domain;

namespace DarkAge.Core.Services
{
    public interface IGameRulesProvider
    {
        GameRules GetRules();
    }
}
