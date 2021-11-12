using UniRx;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class BombController : BaseController, ITickable
    {
        #region Fields

        private readonly Bomb _bomb;
        private readonly IPlantBomb _bombSetter;
        private readonly IPlantBombButton _plantBombButton;
        private readonly IEatApple _eatApple;

        private CompositeDisposable _disposables;
        private int _countOffApplesToEat = 2;
        private int _apples = 0;
        private float _timer;
        private float _exploseTimer;

        #endregion


        #region ClassLifeCycles

        public BombController(
            Bomb bomb, 
            IPlantBomb bombSetter,
            IEatApple eatApple,
            IPlantBombButton plantBombButton)
        {
            _bomb = bomb;
            _bombSetter = bombSetter;
            _eatApple = eatApple;
            _plantBombButton = plantBombButton;
            _disposables = new CompositeDisposable();
        }

        public override void Start()
        {
            _eatApple.OnAppleEat.Subscribe(_ => CheckEatenApples()).AddTo(_disposables);
            _bombSetter.BombIsPLanted.Subscribe(_ => ResetApplesCounter()).AddTo(_disposables);
        }

        public override void Dispose() =>
            _disposables.Clear();

        #endregion


        public void Tick()
        {
            if (_bomb.gameObject.activeInHierarchy)
            {
                _timer += Time.deltaTime;
                _exploseTimer += Time.deltaTime;


                if (_timer >= _bomb.SpeedFireAnimation)
                {
                    _bomb.AnimateFire();
                    _timer = 0;
                }

                if (_exploseTimer >= _bomb.BombTimer)
                    Explose();
            }
        }



        #region Methods
        private void Explose()
        {
            _bomb.OnExplosed();
            _exploseTimer = 0;
            _plantBombButton.SetButtonActive(false);
        }

        private void ResetApplesCounter() =>
            _apples = 0;

        private void CheckEatenApples()
        {
            _apples++;
            if (_apples >= _countOffApplesToEat)
                _plantBombButton.SetButtonActive(true);
            
        }

        #endregion

    }
}
