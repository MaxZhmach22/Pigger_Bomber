using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class MainGameController : BaseController
    {
        private readonly PlayerMovementController _playerMovementController;
        //private readonly EnemiesController _enemiesController;
        //private readonly ShootingController _shootingController;
        //private readonly GameUiController _gameUiController;
        //private readonly PlayerCollisionController _playerCollisionController;
        private readonly Player _player;

        public MainGameController(
             PlayerMovementController playerMovementController,
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
            _player = player;
        }

        public override void Start()
        {
            _player.gameObject.SetActive(true);

            _playerMovementController.Start();
            //_gameLevelViewController.Start();
            //_enemiesController.Start();
            //_shootingController.Start();
            //_gameUiController.Start();
            //_playerCollisionController.Start();
        }

        public override void Dispose()
        {
            _playerMovementController?.Dispose();
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
