using Zenject;

namespace PiggyBomber
{
    internal sealed class StartGameState : GameState
    {
        private MainMenuController _mainMenuController;
        private readonly MainMenuController.Factory _mainMenuControllerFactory;
        private readonly Player _player;

        public StartGameState(MainMenuController.Factory mainMenuControllerFactory) =>
            _mainMenuControllerFactory = mainMenuControllerFactory;

        public override void Start()
        {
            _mainMenuController = _mainMenuControllerFactory.Create();
            _mainMenuController.Start();
        }

        public override void Dispose() =>
            _mainMenuController.Dispose();

        public override void Update() { }


        internal class Factory : PlaceholderFactory<StartGameState>
        {
        }
    }
}
