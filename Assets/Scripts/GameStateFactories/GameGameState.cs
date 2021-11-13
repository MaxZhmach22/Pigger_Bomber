using System;
using Zenject;

namespace PiggerBomber
{
    internal sealed class GameGameState : GameState
    {
        private MainGameController _mainGameController;
        private readonly MainGameController.Factory _mainGameControllerFactory;

        #region ClassLifeCycles

        public GameGameState(MainGameController.Factory gameProcessControllerFactory) =>
             _mainGameControllerFactory = gameProcessControllerFactory;

        public override void Start()
        {
            _mainGameController = _mainGameControllerFactory.Create();
            _mainGameController.Start();
        }

        public override void Dispose() =>
            _mainGameController.Dispose();

        #endregion


        public override void Update() { }

        internal sealed class Factory : PlaceholderFactory<GameGameState>
        {
        }
    }
}
