using Zenject;
using UniRx;

namespace PiggyBomber
{
    internal sealed class MainMenuController : BaseController
    {
        private readonly MainMenuView _mainMenuView;
        private readonly Player _player;

        public MainMenuController(
            MainMenuView mainMenuView,
            Player player)
        {
            _mainMenuView = mainMenuView;
            _player = player;
        }

        public override void Start()
        {
            _player.gameObject.SetActive(false);
            _mainMenuView.gameObject.SetActive(true);

            _mainMenuView.StartGameBtn.OnClickAsObservable()
                .Subscribe(_ => _player.ChangeState(GameStates.Game))
                .AddTo(_mainMenuView);
        }
        public override void Dispose()
        {
            _mainMenuView.gameObject.SetActive(false);
        }

        public class Factory : PlaceholderFactory<MainMenuController>
        {
        }
    }
}