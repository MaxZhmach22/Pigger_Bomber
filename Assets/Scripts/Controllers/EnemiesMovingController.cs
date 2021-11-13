using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class EnemiesMovingController : BaseController, ITickable, IEnemiesMovingController
    {
        #region Fields

        private readonly GridController _gridController;
        private readonly Player _player;

        public DogEnemy DogEnemy;
        public HumanEnemy HumanEnemy;
        private bool _humanSeePlayer;
        private bool _dogSeePlayer;
        private float _humanEnemyTimer;
        private float _dogEnemyTimer;
        private Vector3 _humanLerpPos;
        private Vector3 _dogLerpPos;
        private float _distance = 0.5f;
        private float _timeToMove = 1f;
        private CompositeDisposable _disposables;
        private Vector2Int _startPosHuman =>
            new Vector2Int(_gridController.CurrentGridArray.GetUpperBound(0),
                           _gridController.CurrentGridArray.GetUpperBound(1));
        private Vector2Int _startPosDog =>
            new Vector2Int(_gridController.CurrentGridArray.GetUpperBound(0),
                           _gridController.CurrentGridArray.GetLowerBound(1)); 

        #endregion


        #region ClassLifeCycles

        public EnemiesMovingController(
            DogEnemy dogEnemy,
            HumanEnemy humanEnemy,
            GridController gridController,
            Player player)
        {
            DogEnemy = dogEnemy;
            HumanEnemy = humanEnemy;
            _gridController = gridController;
            _player = player;
            _disposables = new CompositeDisposable();
        }

        public override void Start()
        {
            HumanEnemy.Init(this);
            DogEnemy.Init(this);
            HumanEnemy.gameObject.SetActive(true);
            DogEnemy.gameObject.SetActive(true);
            ResetAll();
            SetStartPoint();
            RandomPointToWalk(_startPosHuman.x, _startPosHuman.y, HumanEnemy);
            RandomPointToWalk(_startPosDog.x, _startPosDog.y, DogEnemy);
        }

        private void ResetAll()
        {
            ResetPath(DogEnemy.Path);
            DogEnemy.PathIndex = 0;
            ResetPath(HumanEnemy.Path);
            HumanEnemy.PathIndex = 0;
            _humanSeePlayer = false;
            _dogSeePlayer = false;
        }
        public override void Dispose()
        {
            HumanEnemy.ResetAllValues();
            DogEnemy.ResetAllValues();
            SetStartPoint();
            _disposables.Clear();
        }


        #endregion


        #region ZenjectUpdateMethods

        public void Tick()
        {
            if (!HumanEnemy.gameObject.activeInHierarchy ||
                !DogEnemy.gameObject.activeInHierarchy)
                return;

            if (_player.CurrentGameState != GameStates.Game)
                return;

            HumanEnemyMoving();
            DogEnemyMoving();
        }

        #endregion


        #region Methods

        private void ResetPath(List<GameObject> path)
        {
            foreach (var gameObject in path)
                GameObject.Destroy(gameObject);
            path.Clear();
        }

        private void RandomPointToWalk(int currentPosX, int currentPosY, BaseEnemy enemy)
        {
            enemy.Path.Clear();
            var randomPoint = _gridController.GenerateRandomPointInGrid();
            _gridController.SetDistance(currentPosX, currentPosY);
            enemy.Path = _gridController.SetPath(randomPoint.x, randomPoint.y);
            if (enemy.PathIndex == enemy.PathIndex - 1)
                RandomPointToWalk(currentPosX, currentPosY, enemy);

            enemy.PathIndex = enemy.Path.Count - 1;
        }

        private void SetStartPoint()
        {
            HumanEnemy.transform.position = _gridController.CurrentGridArray[_startPosHuman.x, _startPosHuman.y].transform.position;
            DogEnemy.transform.position = _gridController.CurrentGridArray[_startPosDog.x, _startPosDog.y].transform.position;
        }

        public Vector3 GetHumanEnemyPosition() => HumanEnemy.transform.position;
        public Vector3 GetDogEnemyPosition() => DogEnemy.transform.position;



        public void SetHumanPath(bool seePlayer)
        {
            if (HumanEnemy.IsDirty)
                return;

            var startPosX = HumanEnemy.Path[HumanEnemy.PathIndex].GetComponent<GridStatView>().x;
            var startPosY = HumanEnemy.Path[HumanEnemy.PathIndex].GetComponent<GridStatView>().y;

            if (seePlayer)
            {
                _humanSeePlayer = true;
                HumanEnemy.Path.Clear();
                HumanEnemy.Path = _gridController.CreateNewPath(startPosX, startPosY, _player.CurrentIndexInArray);
                HumanEnemy.PathIndex = HumanEnemy.Path.Count - 1;
            }
            else
            {
                _humanSeePlayer = false;
                RandomPointToWalk(startPosX, startPosY, HumanEnemy);
            }
        }

        public void SetDogPath(bool seePlayer)
        {
            if (DogEnemy.IsDirty)
                return;

            var startPosX = DogEnemy.Path[DogEnemy.PathIndex].GetComponent<GridStatView>().x;
            var startPosY = DogEnemy.Path[DogEnemy.PathIndex].GetComponent<GridStatView>().y;

            if (seePlayer)
            {
                _dogSeePlayer = true;
                DogEnemy.Path.Clear();
                _gridController.SetDistance(startPosX, startPosY);
                DogEnemy.Path = _gridController.CreateNewPath(startPosX, startPosY, _player.CurrentIndexInArray);
                DogEnemy.PathIndex = DogEnemy.Path.Count - 1;
            }
            else
            {
                _dogSeePlayer = false;
                RandomPointToWalk(startPosX, startPosY, DogEnemy);
            }
        }

        private void HumanEnemyMoving()
        {
            if (HumanEnemy.IsDirty)
            {
                HumanEnemy.Path.Clear();
                HumanEnemy.transform.position = _gridController.CurrentGridArray[_startPosHuman.x, _startPosHuman.y].transform.position;
                RandomPointToWalk(_startPosHuman.x, _startPosHuman.y, HumanEnemy);
                HumanEnemy.IsDirty = false;
            }

            var distance = Vector3.Distance(_player.transform.position, HumanEnemy.transform.position);

            if (distance <= _distance && _humanSeePlayer)
            {
                DogEnemy.gameObject.SetActive(false);
                HumanEnemy.gameObject.SetActive(false);
                _player.ChangeState(GameStates.End);
                return;
            }
            else if (HumanEnemy.PathIndex == 0)
            {
                RandomPointToWalk(
                    HumanEnemy.Path[0].gameObject.GetComponent<GridStatView>().x,
                    HumanEnemy.Path[0].gameObject.GetComponent<GridStatView>().y,
                    HumanEnemy);
                HumanEnemy.ResetAllValues();
            }
            if (!HumanEnemy.IsMoving)
                _humanLerpPos = HumanEnemy.transform.position;

            Move(HumanEnemy, HumanEnemy.Path, ref _humanLerpPos, ref _humanEnemyTimer);
        }

        private void DogEnemyMoving()
        {
            if (DogEnemy.IsDirty)
            {
                DogEnemy.Path.Clear();
                DogEnemy.transform.position = _gridController.CurrentGridArray[_startPosHuman.x, _startPosHuman.y].transform.position;
                RandomPointToWalk(_startPosDog.x, _startPosDog.y, DogEnemy);
                DogEnemy.IsDirty = false;
            }

            var distance = Vector3.Distance(_player.transform.position, DogEnemy.transform.position);

            if (distance <= _distance && _dogSeePlayer)
            {
                DogEnemy.gameObject.SetActive(false);
                HumanEnemy.gameObject.SetActive(false);
                _player.ChangeState(GameStates.End);
                return;
            }
            else if (DogEnemy.PathIndex == 0)
            {
                RandomPointToWalk(
                     DogEnemy.Path[0].gameObject.GetComponent<GridStatView>().x,
                     DogEnemy.Path[0].gameObject.GetComponent<GridStatView>().y,
                     DogEnemy);
                DogEnemy.ResetAllValues();
            }
            if (!DogEnemy.IsMoving)
                _dogLerpPos = DogEnemy.transform.position;

            Move(DogEnemy, DogEnemy.Path, ref _dogLerpPos, ref _dogEnemyTimer);
        }

        private void Move(BaseEnemy enemy, List<GameObject> path, ref Vector3 starLerptPos, ref float timer)
        {
            if (path.Count == 0 || enemy == null)
                return;
            enemy.IsMoving = true;
            timer += Time.deltaTime * enemy.CurrentSpeed;
            var targetPos = path[enemy.PathIndex].transform.position;
            float percentageComplete = timer / _timeToMove;
            enemy.transform.position = Vector3.Lerp(starLerptPos, targetPos, percentageComplete);
            if (timer >= 1)
            {
                enemy.SetSprites(GetSpriteDirection(path, enemy.PathIndex));
                enemy.transform.position = targetPos;
                enemy.PathIndex--;
                enemy.IsMoving = false;
                timer = 0;
            }
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
        #endregion
    }
}
