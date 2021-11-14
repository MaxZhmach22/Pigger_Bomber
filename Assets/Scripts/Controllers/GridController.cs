using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;

namespace PiggerBomber
{
    internal sealed class GridController : BaseController, IGridController
    {
        #region Fields

        public Subject<bool> PlayersNewPosition = new Subject<bool>();
        public GameObject[,] CurrentGridArray { get; private set; }
        public bool FindDistance = false;

        private readonly GridControllerView _gridControllerView;
        private float _scaleX => _gridControllerView.Grid.cellSize.x;
        private Transform _parentTransform;
        private Vector3 _leftBorromLocation = new Vector3(0, 0, 0);
        private int _endX;
        private int _endY;

        #endregion

        #region ClassLifeCycles

        public GridController(GridControllerView gridControllerView) =>
            _gridControllerView = gridControllerView;

        public override void Start()
        {
            CurrentGridArray ??= new GameObject[_gridControllerView.Columns, _gridControllerView.Rows];
            _parentTransform ??= new GameObject("GridArray").transform;
            if (_gridControllerView.GridPointPrefab)
                GenerateGrid();
        }

        public override void Dispose()
        {
            foreach (GameObject gameObject in CurrentGridArray)
                GameObject.Destroy(gameObject);
        }

        #endregion

        #region Methods

        private void GenerateGrid()
        {
            for (int i = 0; i < _gridControllerView.Columns; i++)
            {
                float offset = 0;
                for (int j = 0; j < _gridControllerView.Rows; j++)
                {
                    GameObject obj = GameObject.Instantiate(_gridControllerView.GridPointPrefab, new Vector3(
                        _leftBorromLocation.x + _scaleX * i + offset,
                        _leftBorromLocation.y + j, _leftBorromLocation.z),
                        _gridControllerView.Grid.transform.rotation);
                    obj.transform.SetParent(_parentTransform.transform);
                    foreach (var gridStatView in obj.GetComponents<GridStatBase>())
                    {
                        gridStatView.X = i;
                        gridStatView.Y = j;
                    }
                    offset += _gridControllerView.OffsetX;
                    CurrentGridArray[i, j] = obj;
                }
            }
        }

        public List<GameObject> CreateNewPath(int startX, int startY, Vector2Int playerPos, EnemiesType type)
        {
            switch (type)
            {
                case EnemiesType.Dog:
                    SetDistance(startX, startY, typeof(DogGridStat));
                    return SetPath(playerPos.x, playerPos.y, typeof(DogGridStat));
                case EnemiesType.Human:
                    SetDistance(startX, startY, typeof(HumanGridStat));
                    return SetPath(playerPos.x, playerPos.y, typeof(HumanGridStat));
            }
            return null;
        }

        public Vector2Int GenerateRandomPointInGrid(EnemiesType type, Vector2Int currentPosition)
        {
            Vector2Int randomPos;
            GridStatBase gridStatView;
            do
            {
                var x = UnityEngine.Random.Range(CurrentGridArray.GetLowerBound(0), CurrentGridArray.GetUpperBound(0));
                var y = UnityEngine.Random.Range(CurrentGridArray.GetLowerBound(1), CurrentGridArray.GetUpperBound(1));
                switch (type)
                {
                    case EnemiesType.Dog:
                        gridStatView = CurrentGridArray[x, y].GetComponent<DogGridStat>();
                        break;
                    case EnemiesType.Human:
                        gridStatView = CurrentGridArray[x, y].GetComponent<HumanGridStat>();
                        break;
                    default:
                        gridStatView = null;
                        break;
                }
                randomPos = new Vector2Int(x, y);

            } while (gridStatView == null || currentPosition == randomPos);

            return randomPos;
        }


        public void SetDistance(int startX, int startY, Type type)
        {
            InitialSetup(startX, startY, type);
            for (int step = 1; step < _gridControllerView.Rows * _gridControllerView.Columns; step++)
            {
                foreach (GameObject gameObject in CurrentGridArray)
                {
                    if(gameObject.TryGetComponent(type, out var component))
                    {
                        var gridStat = component as GridStatBase;
                        if (gridStat != null && gridStat.Visited == step - 1)
                            TestFourDirections(gridStat.X, gridStat.Y, step, type);
                    }
                }
            }
        }

        private void InitialSetup(int startX, int startY, Type type)
        {
            foreach (GameObject gameObject in CurrentGridArray)
            {
                if (gameObject.TryGetComponent(type, out var component))
                {
                    var gridStat = component as GridStatBase;
                    if (gridStat != null)
                        gridStat.Visited = -1;
                }
            }
            if(CurrentGridArray[startX, startY].TryGetComponent(type, out var currentPosComp))
            {
                var gridStat = currentPosComp as GridStatBase;
                gridStat.Visited = 0;
            }
        }

        public List<GameObject> SetPath(int endX, int endY, Type type)
        {
            GridStatBase gridStat;
            int step;
            List<GameObject> tempList = new List<GameObject>();
            List<GameObject> path = new List<GameObject>();

            if (CurrentGridArray[endX, endY].TryGetComponent(type, out var currentPosComp))
            {
                gridStat = currentPosComp as GridStatBase;
                if(gridStat.Visited > 0)
                {
                    path.Add(CurrentGridArray[endX, endY]);
                    step = gridStat.Visited - 1;
                }
                else
                {
                    Debug.Log("No PATH!");
                    return null;
                }
                for (; step > -1; step--)
                {
                    if (TestDirection(endX, endY, step, Directions.Up, type))
                        tempList.Add(CurrentGridArray[endX, endY + 1]);
                    if (TestDirection(endX, endY, step, Directions.Right, type))
                        tempList.Add(CurrentGridArray[endX + 1, endY]);
                    if (TestDirection(endX, endY, step, Directions.Down, type))
                        tempList.Add(CurrentGridArray[endX, endY - 1]);
                    if (TestDirection(endX, endY, step, Directions.Left, type))
                        tempList.Add(CurrentGridArray[endX - 1, endY]);

                    GameObject tempObj = FindClosest(CurrentGridArray[endX, endY].transform, tempList);
                    if (tempObj.TryGetComponent(type, out var gridStatView))
                    {
                        var gridStatTemp = gridStatView as GridStatBase;
                        path.Add(tempObj);
                        endX = gridStatTemp.X;
                        endY = gridStatTemp.Y;
                        tempList.Clear();
                    }
                }
            }
            return path;
        }

        private void TestFourDirections(int x, int y, int step, Type type)
        {
            if (TestDirection(x, y, -1, Directions.Up, type))
                SetVisited(x, y + 1, step, type);
            if (TestDirection(x, y, -1, Directions.Right, type))
                SetVisited(x + 1, y, step, type);
            if (TestDirection(x, y, -1, Directions.Down, type))
                SetVisited(x, y - 1, step, type);
            if (TestDirection(x, y, -1, Directions.Left, type))
                SetVisited(x - 1, y, step, type);
        }


        private bool TestDirection(int x, int y, int step, Directions directions, Type type)
        {
            int nearCell = 1;
            switch (directions)
            {
                case Directions.Up:
                    if (y + 1 < _gridControllerView.Rows && CurrentGridArray[x, y + nearCell] && CheckStep(x, y + nearCell, step, type))
                        return true;
                    else
                        return false;
                case Directions.Right:
                    if (x + 1 < _gridControllerView.Columns && CurrentGridArray[x + nearCell, y] && CheckStep(x + nearCell, y, step, type))
                        return true;
                    else
                        return false;
                case Directions.Down:
                    if (y - 1 > -1 && CurrentGridArray[x, y - nearCell] && CheckStep(x, y - nearCell, step, type))
                        return true;
                    else
                        return false;
                case Directions.Left:
                    if (x - 1 > -1 && CurrentGridArray[x - nearCell, y] && CheckStep(x - nearCell, y, step, type))
                        return true;
                    else
                        return false;
            }
            return false;
        }

        private bool CheckStep(int x, int y, int step, Type type)
        {
            CurrentGridArray[x, y].TryGetComponent(type, out var gridStatView);
            if (gridStatView != null)
            {
                var currentGridStat = gridStatView as GridStatBase;
                return currentGridStat.Visited == step;
            }
            else
                return false;
        }

        private void SetVisited(int x, int y, int step, Type type)
        {
            if (!CurrentGridArray[x, y])
                return;

            if (CurrentGridArray[x, y].TryGetComponent(type, out var component))
            {
                var currentGridStatType = component as GridStatBase;
                currentGridStatType.Visited = step;
            }    
        }

        private GameObject FindClosest(Transform targetLoacation, List<GameObject> list)
        {
            float currentDistance = _gridControllerView.Rows * _gridControllerView.Columns;
            int indexNumber = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (Vector3.Distance(targetLoacation.position, list[i].transform.position) < currentDistance)
                {
                    currentDistance = Vector3.Distance(targetLoacation.position, list[i].transform.position);
                    indexNumber = i;
                }
            }
            return list[indexNumber];
        }

        #endregion

    }
}
