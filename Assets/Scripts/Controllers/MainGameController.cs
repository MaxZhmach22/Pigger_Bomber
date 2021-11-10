using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class MainGameController : BaseController
    {
        private readonly PlayerMovementController _playerMovementController;
        private readonly GridController _gridController;
        private readonly TreesViewController _treesViewController;
        private readonly ApplesController _applesController;
        //private readonly EnemiesController _enemiesController;
        //private readonly ShootingController _shootingController;
        //private readonly GameUiController _gameUiController;
        //private readonly PlayerCollisionController _playerCollisionController;
        private readonly Player _player;

        public MainGameController(
             PlayerMovementController playerMovementController,
             GridController gridController,
             TreesViewController treesViewController,
             ApplesController applesController,
             //GameLevelViewController gameLevelViewController,
             //EnemiesController enemiesController,
             //ShootingController shootingController,
             //GameUiController gameUiController,
             //PlayerCollisionController playerCollisionController,
             Player player)
        {
            //_gameLevelViewController = gameLevelViewController;
            //_enemiesController = enemiesController;
            //_shootingController = shootingController;
            //_gameUiController = gameUiController;
            //_playerCollisionController = playerCollisionController;
            
            _playerMovementController = playerMovementController;
            _gridController = gridController;
            _treesViewController = treesViewController;
            _applesController = applesController;
            _player = player;
        }

        public override void Start()
        {
            _player.gameObject.SetActive(true);

            _playerMovementController.Start();
            _gridController.Start();
            _treesViewController.Start();
            _applesController.Start();
            //_gameLevelViewController.Start();
            //_enemiesController.Start();
            //_shootingController.Start();
            //_gameUiController.Start();
            //_playerCollisionController.Start();
        }

        public override void Dispose()
        {
            _playerMovementController?.Dispose();
            _gridController?.Dispose();
            _treesViewController?.Dispose();
            _applesController?.Dispose();
            //_gameLevelViewController?.Dispose();
            //_shootingController?.Dispose();
            //_gameUiController?.Dispose();
            //_enemiesController?.Dispose();
            //_playerCollisionController?.Dispose();
            Debug.Log(nameof(MainGameController) + " Disposed");
        }

        public sealed class Factory : PlaceholderFactory<MainGameController>
        {
        }
    }
}
