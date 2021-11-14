using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class ApplesController : BaseController, IAppleController, IEatApple, ITickable
    {
        #region Fields

        [Inject] private Apple _apple;
        [Inject] private DiContainer _diContainer;

        private readonly TreesViewController _treesViewController;
        private readonly IGridController _gridController;
        private Subject<int> _onAppleEat = new Subject<int>();
        private List<Vector3> _freePositionList;
        private List<GameObject> _activeApples;
        private Transform _parentTransform;
        private float _timerSpawnApple = 3;
        private float _timer;
        private bool _isDisposed;
        private CompositeDisposable _disposables = new CompositeDisposable();
        public ISubject<int> OnAppleEat => _onAppleEat; 

        #endregion


        #region ClassLifeCycles

        public ApplesController(
            TreesViewController treesViewController, 
            IGridController gridController)
        {
            _treesViewController = treesViewController;
            _gridController = gridController;
            _freePositionList = new List<Vector3>();
            _parentTransform = new GameObject("ApplesBasket").transform;
            _activeApples = new List<GameObject>();
        }
        public override void Start()
        {
            _isDisposed = false;
            _parentTransform.gameObject.SetActive(true);
        }

        public override void Dispose()
        {
            _isDisposed = true;
            _parentTransform.gameObject.SetActive(false);
            foreach (var apple in _activeApples)
               GameObject.Destroy(apple);
            _activeApples.Clear();
            _freePositionList.Clear();
            _disposables.Clear();
            _timer = 0;

        }

        #endregion


        #region ZenjectUpdateMethods

        public void Tick()
        {
            if (_isDisposed)
                return;

            _timer += Time.deltaTime;
            if (_timer >= _timerSpawnApple)
            {
                CreateApple();
                _timer = 0;
            }
        }

        #endregion


        #region Methods

        private void CreateApple()
        {
            if (_treesViewController._matureTreesPositionList.Count == 0)
                return;

            var apple = _diContainer.InstantiatePrefab(_apple, _parentTransform);
            _activeApples.Add(apple);
            CheckOldApples();
            foreach (var trees in _treesViewController.MatureTreesPositionList)
            {
                CheckFreePosition(_gridController.CurrentGridArray, trees);
            }
            apple.transform.position = RandomApplePosition();
        }

        private void CheckOldApples()
        {
            if (_activeApples.Count > 5)
            {
                var oldApple = _activeApples[0];
                _activeApples.RemoveAt(0);
                GameObject.Destroy(oldApple);
            }
        }

        private void CheckFreePosition(GameObject[,] gridArray, Vector2Int currentMatureTreePosition)
        {
            _freePositionList.Clear();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var gameObject = gridArray[currentMatureTreePosition.x + i, currentMatureTreePosition.y + j];
                    if (gameObject.TryGetComponent<HumanGridStat>(out var gridStatView))
                        _freePositionList.Add(gridStatView.transform.position);
                }
            }
        }

        private Vector3 RandomApplePosition() =>
            _freePositionList[UnityEngine.Random.Range((int)0, _freePositionList.Count - 1)];

        public void EatApple(Apple apple)
        {
            _onAppleEat.OnNext(apple.Score);
            _activeApples.Remove(apple.gameObject);
        }

        #endregion
    }
}
