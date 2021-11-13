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
        private readonly EnemiesMovingController _enemiesMovingController;

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
            IPlantBombButton plantBombButton,
            EnemiesMovingController enemiesMovingController)
        {
            _bomb = bomb;
            _bombSetter = bombSetter;
            _eatApple = eatApple;
            _plantBombButton = plantBombButton;
            _enemiesMovingController = enemiesMovingController;
            _disposables = new CompositeDisposable();
        }

        public override void Start()
        {
            _eatApple.OnAppleEat.Subscribe(_ => CheckEatenApples()).AddTo(_disposables);
            _bombSetter.BombIsPLanted.Subscribe(_ => ResetApplesCounter()).AddTo(_disposables);
            _bomb.gameObject.SetActive(false);
        }

        public override void Dispose()
        {
            _timer = 0;
            _exploseTimer = 0;
            ResetApplesCounter();
            _disposables.Clear();
        }


        #endregion


        #region ZenjectUpdateMethods

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

        #endregion


        #region Methods

        private void Explose()
        {
            _bomb.OnExplosed();
            _exploseTimer = 0;
            _plantBombButton.SetButtonActive(false);
            if (Vector3.Distance(_enemiesMovingController.GetDogEnemyPosition(), _bomb.transform.position) < _bomb.ExplosionRadius)
                _enemiesMovingController.DogEnemy.GetDirty();
            if (Vector3.Distance(_enemiesMovingController.GetHumanEnemyPosition(), _bomb.transform.position) < _bomb.ExplosionRadius)
                _enemiesMovingController.HumanEnemy.GetDirty();
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
