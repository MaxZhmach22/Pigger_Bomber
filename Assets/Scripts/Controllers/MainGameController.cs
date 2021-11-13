using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class MainGameController : BaseController
    {
        #region Fields

        private readonly PlayerMovementController _playerMovementController;
        private readonly GridController _gridController;
        private readonly TreesViewController _treesViewController;
        private readonly ApplesController _applesController;
        private readonly EnemiesMovingController _enemiesMovingController;
        private readonly BombController _bombController;
        private readonly GameUiController _gameUiController;
        private readonly Player _player;

        #endregion

        #region ClassLifeCycles

        public MainGameController(
             PlayerMovementController playerMovementController,
             GridController gridController,
             TreesViewController treesViewController,
             ApplesController applesController,
             EnemiesMovingController enemiesMovingController,
             BombController bombController,
             GameUiController gameUiController,
             Player player)
        {
            _playerMovementController = playerMovementController;
            _gridController = gridController;
            _treesViewController = treesViewController;
            _applesController = applesController;
            _enemiesMovingController = enemiesMovingController;
            _bombController = bombController;
            _gameUiController = gameUiController;
            _player = player;
        }

        public override void Start()
        {
            _player.gameObject.SetActive(true);
            _gridController.Start();
            _treesViewController.Start();
            _applesController.Start();
            _playerMovementController.Start();
            _enemiesMovingController.Start();
            _bombController.Start();
            _gameUiController.Start();
        }

        public override void Dispose()
        {
            _playerMovementController?.Dispose();
            _gridController?.Dispose();
            _treesViewController?.Dispose();
            _applesController?.Dispose();
            _enemiesMovingController?.Dispose();
            _bombController?.Dispose();
            _gameUiController?.Dispose();
            Debug.Log(nameof(MainGameController) + " Disposed");
        }

        #endregion


        public sealed class Factory : PlaceholderFactory<MainGameController>
        {
        }
    }
}
