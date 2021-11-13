using UnityEngine;
using Zenject;
using UniRx;

namespace PiggerBomber
{
    internal sealed class LooseGameController : GameState
    {
        #region Fields

        private CompositeDisposable _disposables;
        private readonly LooseMenuView _looseMenuView;
        private readonly Player _player;

        #endregion


        #region ClassLifeCycles

        public LooseGameController(
            LooseMenuView looseMenuView,
            Player player)
        {
            _looseMenuView = looseMenuView;
            _player = player;
            _disposables = new CompositeDisposable();
        }

        public override void Start()
        {
            _player.gameObject.SetActive(false);
            Time.timeScale = 0f;
            _looseMenuView.gameObject.SetActive(true);
            _looseMenuView.QuitButton
                .OnClickAsObservable()
                .Subscribe(_ => Application.Quit()).AddTo(_disposables);
            _looseMenuView.TryAgainButton
                .OnClickAsObservable()
                .Subscribe(_ => _player.ChangeState(GameStates.Start))
                .AddTo(_disposables);
            SetScoreTxt();
        }

        public override void Dispose()
        {
            _disposables.Clear();
            _looseMenuView.gameObject.SetActive(false);
            Debug.Log(nameof(LooseGameController) + " Is Disposed");
        }
            
        #endregion


        #region Methods

        private void SetScoreTxt() =>
            _looseMenuView.ScoreTxt.text = PlayerPrefs.GetString("Score");

        public override void Update() { }
  
        #endregion


        public sealed class Factory : PlaceholderFactory<LooseGameController>
        {
        }
    }
}

