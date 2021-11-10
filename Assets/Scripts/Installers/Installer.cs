using System;
using UnityEngine;
using Zenject;

namespace PiggyBomber
{
    public class Installer : MonoInstaller
    {
        [SerializeField] private Transform _placeForUi;
        [SerializeField] private Player _player;
        [SerializeField] private DynamicJoystick _dynamicJoystick;
        [SerializeField] private MainMenuView _mainMenuView;
        //[SerializeField] private LevelView _levelView;
        [SerializeField] private Grid _grid;

        public override void InstallBindings()
        {
            Container.Bind<Grid>().WithId("Grid").FromInstance(_grid).AsSingle();
            Container.Bind<Player>().FromInstance(_player).AsSingle();
            InstalGameStateFactories();
            InputSystemInstaller();
            MainGameControllerBindings();
            MainMenuControllerBindings();
        }

        private void MainGameControllerBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerMovementController>().AsSingle();
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