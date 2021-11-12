using Zenject;
using UniRx;
using UnityEngine;

namespace PiggerBomber
{
    internal sealed class GameUiController : BaseController, IPlantBombButton
    {
        private readonly GameUiView _gameUiView;
        private readonly IEatApple _eatApple;
        private readonly Player _player;
        private readonly CompositeDisposable _disposables;

        private int _score = 0;

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

            _gameUiView.BombPlantButton.OnClickAsObservable()
                .Subscribe(_ => _player.ChangeState(GameStates.Game))
                .AddTo(_gameUiView);
            _gameUiView.BackToMenuButton.OnClickAsObservable().Subscribe(_ => _player.ChangeState(GameStates.Start)).AddTo(_gameUiView);
            _gameUiView.ScoreTxt.text = _score.ToString();
            _eatApple.OnAppleEat.Subscribe(value => SetNewScore(value)).AddTo(_disposables);
        }

        private void SetNewScore(int value)
        {
            _score += value;
            _gameUiView.ScoreTxt.text = _score.ToString(); 
        }

        public override void Dispose()
        {
            PlayerPrefs.SetString("Score", _gameUiView.ScoreTxt.text);
            _gameUiView.gameObject.SetActive(false);
            _disposables.Clear();
        }

        public void SetButtonActive(bool status) =>
            _gameUiView.BombPlantButton.interactable = status;
     
        public class Factory : PlaceholderFactory<MainMenuController>
        {
        }
    }
}