using System;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    public class Installer : MonoInstaller
    {
        [SerializeField] private Transform _placeForUi;
        [SerializeField] private Player _player;
        [SerializeField] private DynamicJoystick _dynamicJoystick;
        [SerializeField] private MainMenuView _mainMenuView;
        [SerializeField] private LevelView _levelView;
        [SerializeField] private GridControllerView _gridControllerView;
        [SerializeField] private AppleTree _appleTree;
        [SerializeField] private Apple _apple;
        [SerializeField] private Grid _grid;

        public override void InstallBindings()
        {
            Container.Bind<Player>().FromInstance(_player).AsSingle();
            InstalGameStateFactories();
            InputSystemInstaller();
            MainMenuControllerBindings();
            MainGameControllerBindings();
        }

        private void MainGameControllerBindings()
        {
            var parentObj = new GameObject("GridView");
            Container.Bind<IGridController>().To<GridController>().WhenInjectedInto<PlayerMovementController>();
            Container.BindInterfacesAndSelfTo<PlayerMovementController>().AsSingle();
            Container.Bind<GridController>().AsCached();
            Container.Bind<IGridController>().To<GridController>().WhenInjectedInto<TreesViewController>();
            Container.Bind<TreesViewController>().AsCached();
            Container.Bind<AppleTree>().FromInstance(_appleTree).AsSingle();

            Container.Bind<ITreesViewController>().To<TreesViewController>().WhenInjectedInto<ApplesController>();
            Container.BindInterfacesAndSelfTo<ApplesController>().AsSingle();
            Container.Bind<Apple>().FromInstance(_apple).AsSingle();

            var gridControllerView = Container.InstantiatePrefabForComponent<GridControllerView>(
               _gridControllerView, parentObj.transform);
            Container.Bind<GridControllerView>().FromInstance(gridControllerView).AsSingle();
            var grid = Container.InstantiatePrefabForComponent<Grid>(
              _grid, parentObj.transform);
            Container.Bind<Grid>().FromInstance(grid).AsSingle();
            var levelView = Container.InstantiatePrefabForComponent<LevelView>(
               _levelView, new GameObject("LevelView").transform);
            Container.Bind<LevelView>().FromInstance(levelView).AsSingle();
            var gridStatView = Container.InstantiatePrefabForComponent<Grid>(
              _grid, parentObj.transform);
        }

        private void InputSystemInstaller()
        {
            var joystic = Container.InstantiatePrefabForComponent<DynamicJoystick>(_dynamicJoystick, _placeForUi);
            Container.Bind<DynamicJoystick>().FromInstance(joystic).AsSingle();
        }

        private void InstalGameStateFactories()
        {
            Container.Bind<GameStateFactory>().AsSingle();

            Container.BindFactory<StartGameState, StartGameState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<MainMenuController, MainMenuController.Factory>().WhenInjectedInto<StartGameState>();

            Container.BindFactory<GameGameState, GameGameState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<MainGameController, MainGameController.Factory>().WhenInjectedInto<GameGameState>();

        }

        private void MainMenuControllerBindings()
        {
            var mainMenuView = Container.InstantiatePrefabForComponent<MainMenuView>(
                _mainMenuView, _placeForUi);
            Container.Bind<MainMenuView>().FromInstance(mainMenuView).AsSingle();
        }

    }
}