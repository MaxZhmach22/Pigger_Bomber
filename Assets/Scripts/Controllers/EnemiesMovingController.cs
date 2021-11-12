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
        private readonly GridController _gridController;
        private readonly Player _player;
        private bool _humanSeePlayer;
        private bool _dogSeePlayer;

        private float _timeToMove = 1f;
        private CompositeDisposable _disposables;
   
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
            GridController gridController,
            Player player)
        {
            _dogEnemy = dogEnemy;
            _humanEnemy = humanEnemy;
            _gridController = gridController;
            _player = player;
            _disposables = new CompositeDisposable();
        }

        public override void Start()
        {
            _humanEnemy.SeePlayer.Subscribe(boolean => SetHumanPath(boolean)).AddTo(_disposables);
            _dogEnemy.SeePlayer.Subscribe(boolean => SetDogPath(boolean)).AddTo(_disposables);
            SetStartPoint();
            RandomPointToWalk(_startPosHuman.x, _startPosHuman.y, _humanEnemy);
            RandomPointToWalk(_startPosDog.x, _startPosDog.y, _dogEnemy);
        }

        private void RandomPointToWalk(int currentPosX, int currentPosY, BaseEnemy enemy)
        {
            enemy.Path.Clear();
            var randomPoint = _gridController.GenerateRandomPointInGrid();
            _gridController.SetDistance(currentPosX, currentPosY);
            enemy.Path = _gridController.SetPath(randomPoint.x, randomPoint.y);
            enemy.PathIndex = enemy.Path.Count - 1;
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

        private void SetHumanPath(bool seePlayer)
        {
            var startPosX = _humanEnemy.Path[_humanEnemy.PathIndex].GetComponent<GridStatView>().x;
            var startPosY = _humanEnemy.Path[_humanEnemy.PathIndex].GetComponent<GridStatView>().y;

            if (seePlayer)
            {
                _humanSeePlayer = true;
                _humanEnemy.Path.Clear();
                _gridController.SetDistance(startPosX, startPosY);
                _humanEnemy.Path = _gridController.SetPath(_player.CurrentIndexInArray.x, _player.CurrentIndexInArray.y);
                _humanEnemy.PathIndex = _humanEnemy.Path.Count - 1;
            }
            else
            {
                _humanSeePlayer = false;
                RandomPointToWalk(startPosX, startPosY, _humanEnemy);
            }
        }

        private void SetDogPath(bool seePlayer)
        {
            var startPosX = _dogEnemy.Path[_dogEnemy.PathIndex].GetComponent<GridStatView>().x;
            var startPosY = _dogEnemy.Path[_dogEnemy.PathIndex].GetComponent<GridStatView>().y;

            if (seePlayer)
            {
                _dogSeePlayer = true; 
                _dogEnemy.Path.Clear();
                _gridController.SetDistance(startPosX, startPosY);
                _dogEnemy.Path = _gridController.SetPath(_player.CurrentIndexInArray.x, _player.CurrentIndexInArray.y);
                _dogEnemy.PathIndex = _dogEnemy.Path.Count - 1;
            }
            else
            {
                _dogSeePlayer = false;
                RandomPointToWalk(startPosX, startPosY, _dogEnemy);
            }
                
        }

        public void Tick()
        {
            if (!_humanEnemy.gameObject.activeInHierarchy ||
                !_dogEnemy.gameObject.activeInHierarchy)
                return;

            HumanEnemyMoving();
            DogEnemyMoving();
        }

        private void HumanEnemyMoving()
        {
            if (_humanEnemy.PathIndex == 0)
            {
                if (_humanSeePlayer)
                {
                    Debug.Log("You Loose");
                    return;
                }
                else
                {
                    RandomPointToWalk(
                   _humanEnemy.Path[0].gameObject.GetComponent<GridStatView>().x,
                   _humanEnemy.Path[0].gameObject.GetComponent<GridStatView>().y,
                   _humanEnemy);
                }
            }
            if (!_humanEnemy.IsMoving)
            {
                MainThreadDispatcher.StartUpdateMicroCoroutine(Move(_humanEnemy, _humanEnemy.Path));
                _humanEnemy.IsMoving = true;
            }
        }

        private void DogEnemyMoving()
        {
            if (_dogEnemy.PathIndex == 0)
            {
                if (_dogSeePlayer)
                {
                    Debug.Log("You Loose");
                    return;
                }
                else
                {
                    RandomPointToWalk(
                   _dogEnemy.Path[0].gameObject.GetComponent<GridStatView>().x,
                   _dogEnemy.Path[0].gameObject.GetComponent<GridStatView>().y,
                   _dogEnemy);
                }
            }
            if (!_dogEnemy.IsMoving)
            {
                MainThreadDispatcher.StartUpdateMicroCoroutine(Move(_dogEnemy, _dogEnemy.Path));
                _dogEnemy.IsMoving = true;
            }
        }

        private IEnumerator Move(BaseEnemy enemy, List<GameObject> path)
        {
            Vector3 originPos;
            Vector3 targetPos;
            float elapsedTime = 0;
            originPos = enemy.gameObject.transform.position;
            targetPos = path[enemy.PathIndex].transform.position;
            while (elapsedTime * enemy.CurrentSpeed < _timeToMove)
            {
                enemy.transform.position = Vector3.Lerp(originPos, targetPos, (elapsedTime * enemy.CurrentSpeed / _timeToMove));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
           
            enemy.SetSprites(GetSpriteDirection(path, enemy.PathIndex));
            enemy.transform.position = targetPos;
            enemy.PathIndex--;
            enemy.IsMoving = false;
        }

        private Directions GetSpriteDirection(List<GameObject> path, int index)
        {
            if (index < 0 || path.Count == 0)
                return Directions.Left;

            if (path[index].gameObject.GetComponent<GridStatView>().x < path[index - 1].gameObject.GetComponent<GridStatView>().x)
                return Directions.Right;
            if (path[index].gameObject.GetComponent<GridStatView>().x > path[index - 1].gameObject.GetComponent<GridStatView>().x)
                return Directions.Left;
            if (path[index].gameObject.GetComponent<GridStatView>().y > path[index - 1].gameObject.GetComponent<GridStatView>().y)
                return Directions.Down;
            if (path[index].gameObject.GetComponent<GridStatView>().y < path[index - 1].gameObject.GetComponent<GridStatView>().y)
                return Directions.Up;

            return default;
        } 
    }
}
