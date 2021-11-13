using Zenject;
using UniRx;
using UnityEngine;

namespace PiggerBomber
{
    internal sealed class GameUiController : BaseController, IPlantBombButton
    {
        #region Fields

        private readonly GameUiView _gameUiView;
        private readonly IEatApple _eatApple;
        private readonly Player _player;
        private readonly CompositeDisposable _disposables;

        private int _applesNeedToEat = 2;
        private int _score = 0;

        #endregion


        #region ClassLifeCycles

        public GameUiController(
            GameUiView gameUiView,
            IEatApple eatApple,
            Player player)
        {
            _gameUiView = gameUiView;
            _eatApple = eatApple;
            _player = player;
            _disposables = new CompositeDisposable();
        }

        public override void Start()
        {
            _gameUiView.gameObject.SetActive(true);
            _gameUiView.BombPlantButton.interactable = false;

            _gameUiView.BombPlantButton
                .OnClickAsObservable()
                .Subscribe(_ => _player.PlantBomb())
                .AddTo(_disposables);
            _gameUiView.BackToMenuButton.
                OnClickAsObservable()
                .Subscribe(_ => _player.ChangeState(GameStates.Start))
                .AddTo(_disposables);
            _gameUiView.ScoreTxt.text = _score.ToString();
            _gameUiView.ApplesNeddToEatCount.text = _applesNeedToEat.ToString();
            _eatApple.OnAppleEat
                .Subscribe(value => SetNewScore(value))
                .AddTo(_disposables);
        }
        public override void Dispose()
        {
            PlayerPrefs.SetString("Score", _gameUiView.ScoreTxt.text);
            _score = 0;
            _applesNeedToEat = 2;
            _gameUiView.gameObject.SetActive(false);
            _disposables.Clear();
        }

        #endregion


        #region Methods

        private void SetNewScore(int value)
        {
            _score += value;
            _gameUiView.ScoreTxt.text = _score.ToString();
            _applesNeedToEat--;
            _gameUiView.ApplesNeddToEatCount.text = _applesNeedToEat.ToString();
            if (_applesNeedToEat == 0)
            {
                _gameUiView.ApplesNeddToEatCount.text = _applesNeedToEat.ToString();
                _applesNeedToEat = 2;
            }
        }

        public void SetButtonActive(bool status) =>
            _gameUiView.BombPlantButton.interactable = status;

        #endregion


        public class Factory : PlaceholderFactory<MainMenuController>
        {
        }
    }
}