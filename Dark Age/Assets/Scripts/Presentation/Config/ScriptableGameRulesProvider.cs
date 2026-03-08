using DarkAge.Core.Domain;
using DarkAge.Core.Services;

namespace DarkAge.Presentation.Config
{
    public sealed class ScriptableGameRulesProvider : IGameRulesProvider
    {
        private readonly GameBalanceConfig _gameBalanceConfig;
        private readonly WorldConfig _worldConfig;

        public ScriptableGameRulesProvider(GameBalanceConfig gameBalanceConfig, WorldConfig worldConfig)
        {
            _gameBalanceConfig = gameBalanceConfig;
            _worldConfig = worldConfig;
        }

        public GameRules GetRules()
        {
            return new GameRules(
                _gameBalanceConfig.CreateStartingWallet(),
                _gameBalanceConfig.CreateProductionRules(),
                _gameBalanceConfig.CreateTaskDefinitions(),
                _worldConfig.CreateBounds(),
                _worldConfig.CellSize);
        }
    }
}
