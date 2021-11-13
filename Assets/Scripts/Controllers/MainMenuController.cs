using Zenject;
using UniRx;
using UnityEngine;

namespace PiggerBomber
{
    internal sealed class MainMenuController : BaseController
    {
        #region Fields

        private CompositeDisposable _disposables;
        private readonly MainMenuView _mainMenuView;
        private readonly Player _player; 

        #endregion


        #region ClassLifeCycles

        public MainMenuController(
            MainMenuView mainMenuView,
            Player player)
        {
            _mainMenuView = mainMenuView;
            _player = player;
            _disposables = new CompositeDisposable();
        }

        public override void Start()
        {
            _player.gameObject.SetActive(false);
            _mainMenuView.gameObject.SetActive(true);

            _mainMenuView.StartGameBtn.OnClickAsObservable()
                .Subscribe(_ => _player.ChangeState(GameStates.Game))
                .AddTo(_disposables);
            _mainMenuView.QuitButton
                .OnClickAsObservable()
                .Subscribe(_ => Application.Quit()).AddTo(_disposables);
        }

        public override void Dispose()
        {
            _disposables.Clear();
            _mainMenuView.gameObject.SetActive(false);
        }

        #endregion


        public class Factory : PlaceholderFactory<MainMenuController>
        {
        }
    }
}