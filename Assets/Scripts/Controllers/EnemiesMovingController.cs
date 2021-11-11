using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class EnemiesMovingController : BaseController, ITickable
    {
        private readonly DogEnemy _dogEnemy;
        private readonly HumanEnemy _humanEnemy;
        private readonly ITreesViewController _treesViewController;
        private readonly GridController _gridController;
        private readonly Player _player;

        private Vector3 _origPosition;
        private Vector3 _targetPosition; 
        private float _timeToMove = 0.5f;
        private bool _isMoving;
        private CompositeDisposable _disposables;

        private List<GameObject> _humanPath = new List<GameObject>();
        private List<GameObject> _dogPath = new List<GameObject>();

        private Vector2Int _startPosHuman =>
            new Vector2Int(_gridController.CurrentGridArray.GetUpperBound(0),
                           _gridController.CurrentGridArray.GetUpperBound(1));

        private Vector2Int _startPosDog =>
            new Vector2Int(_gridController.CurrentGridArray.GetUpperBound(0),
                           _gridController.CurrentGridArray.GetLowerBound(1));

        #region ClassLifeCycles

        public EnemiesMovingController(
            DogEnemy dogEnemy,
            HumanEnemy humanEnemy,
            ITreesViewController treesViewController,
            GridController gridController,
            Player player)
        {
            _dogEnemy = dogEnemy;
            _humanEnemy = humanEnemy;
            _treesViewController = treesViewController;
            _gridController = gridController;
            _player = player;
            _disposables = new CompositeDisposable();
        }

        public override void Start()
        {
            _gridController.PlayersNewPosition.Subscribe(_ => SetPath()).AddTo(_disposables);
            SetStartPoint();
            Debug.Log(_gridController.CurrentGridArray);
        }

        private void SetStartPoint()
        {
            _humanEnemy.transform.position = _gridController.CurrentGridArray[_startPosHuman.x, _startPosHuman.y].transform.position;
            _dogEnemy.transform.position = _gridController.CurrentGridArray[_startPosDog.x, _startPosDog.y].transform.position;
        }

        public override void Dispose()
        {
            _disposables.Clear();
        }

        #endregion

        private void SetPath()
        {
            _humanPath.Clear();
            _dogPath.Clear();
            _humanPath = _gridController.CreateNewPath()
        }

        public void Tick()
        {
            if (_humanEnemy.gameObject.activeInHierarchy)
            {
                MainThreadDispatcher.StartUpdateMicroCoroutine(Move(nextPos, key));
            }
        }

        private void Move(List<GameObject> path)
        {
            foreach (var item in collection)
            {

            }
        }

        private IEnumerator Move(GameObject enemy)
        {
            float elapsedTime = 0;
            _origPosition = enemy.transform.position;
            _targetPosition = direction;
            while (elapsedTime < _timeToMove)
            {
                enemy.transform.position = Vector3.Lerp(_origPosition, _targetPosition, (elapsedTime / _timeToMove));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            enemy.transform.position = _targetPosition;
            _isMoving = false;
        }

        private void MoveToNextPos(int x, int y)
        {
            _isMoving = true;
            var key = new Vector2Int(_player.CurrentIndexInArray.x + x, _player.CurrentIndexInArray.y + y);
            _treesViewController.FreePositionInGrid.TryGetValue(key, out var gameObject);
            if (gameObject != null)
            {
                var nextPos = gameObject.transform.position;
                MainThreadDispatcher.StartUpdateMicroCoroutine(Move(nextPos, key));
            }

        }

    }
}
