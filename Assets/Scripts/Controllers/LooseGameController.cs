using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class LooseGameController : GameState
    {
        #region Fields

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
        }

        public override void Start()
        {
            _player.gameObject.SetActive(false);
            Time.timeScale = 0f;
            _looseMenuView.gameObject.SetActive(true);
            _looseMenuView.QuitButton.onClick.AddListener(() => Application.Quit());
            _looseMenuView.TryAgainButton.onClick.AddListener(() => _player.ChangeState(GameStates.Game));
            SetScoreTxt();
        }


        public override void Dispose()
        {
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

