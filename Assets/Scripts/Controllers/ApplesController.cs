using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class ApplesController : BaseController, IAppleController
    {

        [Inject] private Apple _apple;
        [Inject] private DiContainer _diContainer; 

        private readonly TreesViewController _treesViewController;
        private Subject<int> _onAppleEat = new Subject<int>(); 
        private List<Vector3> _freePositionList;
        private List<GameObject> _activeApples;
        private Transform _parentTransform;
        private float _timerSpawnApple = 3;
        private float _timer;

        public ISubject<int> OnAppleEat => _onAppleEat;

        #region ClassLifeCycles

        public ApplesController(TreesViewController treesViewController)
        {
            _treesViewController = treesViewController;
            _freePositionList = new List<Vector3>();
            _parentTransform = new GameObject("ApplesBasket").transform;
            _activeApples = new List<GameObject>();
        }
        public override void Start()
        {
            MainThreadDispatcher.StartFixedUpdateMicroCoroutine(SpawnApple());
        }

        public override void Dispose()
        {
            GameObject.Destroy(_parentTransform);
            foreach (var apple in _activeApples)
            {
                GameObject.Destroy(apple);
            }
            _activeApples.Clear();
            _freePositionList.Clear();
        }

        #endregion

        private IEnumerator SpawnApple()
        {
            while (true)
            {
                _timer += Time.deltaTime;
                yield return null;

                if (_timer < _timerSpawnApple)
                    continue;

                CreateApple();

            }
        }

        private void CreateApple()
        {
            if (_treesViewController._matureTreesPositionList.Count == 0)
                return;

            var apple = _diContainer.InstantiatePrefab(_apple, _parentTransform);
            _activeApples.Add(apple);
            foreach (var trees in _treesViewController.MatureTreesPositionList)
            {
                CheckFreePosition(_treesViewController.CurrentGridArray,trees);
            }
            apple.transform.position = RandomApplePosition();
            _timer = 0;
        }

        private void CheckFreePosition(GameObject[,] gridArray, Vector2Int currentMatureTreePosition)
        {
            _freePositionList.Clear();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var gameObject = gridArray[currentMatureTreePosition.x + i, currentMatureTreePosition.y + j];
                    if (gameObject.TryGetComponent<GridStatView>(out var gridStatView))
                        _freePositionList.Add(gridStatView.transform.position);
                }
            }
        }

        private Vector3 RandomApplePosition() =>
            _freePositionList[UnityEngine.Random.Range((int)0, _freePositionList.Count-1)];

        public void EatApple(Apple apple)
        {
            _onAppleEat.OnNext(apple.Score);
            _activeApples.Remove(apple.gameObject);
        }
    }
}
