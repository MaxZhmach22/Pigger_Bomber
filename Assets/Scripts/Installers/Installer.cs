using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class Installer : MonoInstaller
    {
        [SerializeField] private Player _player;
        [SerializeField] private DynamicJoystick _dynamicJoystick;
        [SerializeField] private LevelView _levelView;
        [SerializeField] private GridControllerView _gridControllerView;
        [SerializeField] private AppleTree _appleTree;
        [SerializeField] private Apple _apple;
        [SerializeField] private Grid _grid;
        [SerializeField] private DogEnemy _dogEnemy;
        [SerializeField] private HumanEnemy _humanEnemy;
        [SerializeField] private Bomb _bomb;

        [Header("Ui Views")]
        [SerializeField] private Transform _placeForUi;
        [SerializeField] private MainMenuView _mainMenuView;
        [SerializeField] private GameUiView _gameUiView;
        [SerializeField] private LooseMenuView _looseMenuView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Player>().FromInstance(_player).AsSingle();
            InstalGameStateFactories();
            InputSystemInstaller();
            MainMenuControllerBindings();
            LooseMenuControllerBindings();
            GameUiControllerBindings();
            MainGameControllerBindings();
        }

        private void InstalGameStateFactories()
        {
            Container.Bind<GameStateFactory>().AsSingle();
            Container.BindFactory<StartGameState, StartGameState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<MainMenuController, MainMenuController.Factory>().WhenInjectedInto<StartGameState>();
            Container.BindFactory<GameGameState, GameGameState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<MainGameController, MainGameController.Factory>().WhenInjectedInto<GameGameState>();
            Container.BindFactory<EndGameState, EndGameState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<LooseGameController, LooseGameController.Factory>().WhenInjectedInto<EndGameState>();

        }

        private void InputSystemInstaller()
        {
            var joystic = Container.InstantiatePrefabForComponent<DynamicJoystick>(_dynamicJoystick, _placeForUi);
            Container.Bind<DynamicJoystick>().FromInstance(joystic).AsSingle();
        }

        private void MainMenuControllerBindings()
        {
            var mainMenuView = Container.InstantiatePrefabForComponent<MainMenuView>(
                _mainMenuView, _placeForUi);
            Container.Bind<MainMenuView>().FromInstance(mainMenuView).AsSingle();
        }

        private void LooseMenuControllerBindings()
        {
           var looseMenuView = Container.InstantiatePrefabForComponent<LooseMenuView>(
               _looseMenuView, _placeForUi);
           Container.Bind<LooseMenuView>().FromInstance(looseMenuView).AsSingle();
        }

        private void GameUiControllerBindings()
        {
            var gameUiView = Container.InstantiatePrefabForComponent<GameUiView>(
                _gameUiView, _placeForUi);
            Container.Bind<GameUiView>().FromInstance(gameUiView).AsSingle();
        }



        private void MainGameControllerBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerMovementController>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameUiController>().AsSingle();
            GridControllerBindings();
            TreesAndApplesControlBindings();
            EnemiesControllerBindings();
            BombControllerBindings();

        }

        private void GridControllerBindings()
        {
            var parentObj = new GameObject("GridView");
            Container.BindInterfacesAndSelfTo<GridController>().AsSingle();
            var gridControllerView = Container.InstantiatePrefabForComponent<GridControllerView>(
               _gridControllerView, parentObj.transform);
            Container.Bind<GridControllerView>().FromInstance(gridControllerView).AsSingle();
            var grid = Container.InstantiatePrefabForComponent<Grid>(
              _grid, parentObj.transform);
            Container.Bind<Grid>().FromInstance(grid).AsSingle();
            var levelView = Container.InstantiatePrefabForComponent<LevelView>(
               _levelView, new GameObject("LevelView").transform); Container.Bind<LevelView>().FromInstance(levelView).AsSingle();
            var gridStatView = Container.InstantiatePrefabForComponent<Grid>(
              _grid, parentObj.transform);
        }

        private void TreesAndApplesControlBindings()
        {
            Container.BindInterfacesAndSelfTo<ApplesController>().AsSingle();
            Container.BindInterfacesAndSelfTo<TreesViewController>().AsSingle();
            Container.Bind<Apple>().FromInstance(_apple).AsSingle();
            Container.Bind<AppleTree>().FromInstance(_appleTree).AsSingle();
        }

        private void EnemiesControllerBindings()
        {
            var enemies = new GameObject("Enemies").transform;
            Container.BindInterfacesAndSelfTo<EnemiesMovingController>().AsSingle();
            var dogEnemy = Container.InstantiatePrefabForComponent<DogEnemy>(_dogEnemy, enemies);
            var humanEnemy = Container.InstantiatePrefabForComponent<HumanEnemy>(_humanEnemy, enemies);
            Container.Bind<DogEnemy>().FromInstance(dogEnemy).AsSingle();
            Container.Bind<HumanEnemy>().FromInstance(humanEnemy).AsSingle();
        }

        private void BombControllerBindings()
        {
            Container.BindInterfacesAndSelfTo<BombController>().AsSingle();
            var bomb = Container.InstantiatePrefabForComponent<Bomb>(_bomb);
            Container.Bind<Bomb>().FromInstance(bomb).AsSingle();
        }
    }
}