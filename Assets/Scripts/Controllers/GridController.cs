using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace PiggerBomber
{
    internal sealed class GridController : BaseController, IGridController
    {
        private readonly GridControllerView _gridControllerView;

        private int _startX;
        private int _startY;
        private int _endX;
        private int _endY;

        public bool FindDistance = false;

        private Transform _parentTransform;
        private Vector3 _leftBorromLocation = new Vector3(0, 0, 0);
        private float _scaleX => _gridControllerView.Grid.cellSize.x;
        public GameObject[,] CurrentGridArray { get; private set; }
        public IReadOnlyList<GameObject> Path => _path;
        
        private List<GameObject> _path = new List<GameObject>();
        public Subject<bool> PlayersNewPosition = new Subject<bool>();

        public GridController(GridControllerView gridControllerView)
        {
            _gridControllerView = gridControllerView;
        } 

        public override void Start()
        {
            CurrentGridArray = new GameObject[_gridControllerView.Columns, _gridControllerView.Rows];
            _parentTransform = new GameObject("GridArray").transform;
            if (_gridControllerView.GridPointPrefab)
                GenerateGrid();
        }
        public override void Dispose()
        {
            foreach (GameObject gameObject in CurrentGridArray)
                gameObject.SetActive(false);
            Array.Clear(CurrentGridArray, 0, CurrentGridArray.Length);
            _path.Clear();
            _parentTransform.gameObject.SetActive(false);
        }

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
                    obj.GetComponent<GridStatView>().x = i;
                    obj.GetComponent<GridStatView>().y = j;
                    offset += _gridControllerView.OffsetX;
                    CurrentGridArray[i, j] = obj;
                }
            }
        }

        public void SetNewPath(Vector2Int playerPosition)
        {
            _endX = playerPosition.x;
            _endY = playerPosition.y;
            PlayersNewPosition.OnNext(true);
        }

        public List<GameObject> CreateNewPath(int startX, int startY)
        {
            SetDistance(startX, startY);
            return SetPath(_endX, _endY);
        }

        public Vector2Int GenerateRandomPointInGrid()
        {
            Vector2Int randomPos;
            GridStatView gridStatView;
            do
            {
                var x = UnityEngine.Random.Range(CurrentGridArray.GetLowerBound(0), CurrentGridArray.GetUpperBound(0));
                var y = UnityEngine.Random.Range(CurrentGridArray.GetLowerBound(1), CurrentGridArray.GetUpperBound(1));
                gridStatView = CurrentGridArray[x, y].GetComponent<GridStatView>();
                randomPos = new Vector2Int(x, y);
            } while (gridStatView == null);

            return randomPos;
        }

        public void SetDistance(int startX, int startY)
        {
            InitialSetup(startX, startY);
            for (int step = 1; step < _gridControllerView.Rows * _gridControllerView.Columns; step++)
            {
                foreach (GameObject gameObject in CurrentGridArray) 
                {
                   var gridStatView = gameObject.GetComponent<GridStatView>();
                    if(gridStatView != null && gridStatView.visited == step - 1)
                      TestFourDirections(gridStatView.x, gridStatView.y, step);
                }
            }
        }

        private void InitialSetup(int startX, int startY)
        {
            foreach (GameObject gameObject in CurrentGridArray)
            {
                gameObject.TryGetComponent<GridStatView>(out var gridStatView);
                if (gridStatView != null)
                    gridStatView.visited = -1; 
            }
            CurrentGridArray[startX, startY].GetComponent<GridStatView>().visited = 0;
        }

        public List<GameObject> SetPath(int endX, int endY)
        {
            int step;
            List<GameObject> tempList = new List<GameObject>();
            List<GameObject> path = new List<GameObject>();
            if(CurrentGridArray[endX, endY] && CurrentGridArray[endX, endY].GetComponent<GridStatView>().visited > 0)
            {
                path.Add(CurrentGridArray[endX, endY]);
                step = CurrentGridArray[endX, endY].GetComponent<GridStatView>().visited - 1;
            }
            else
            {
                Debug.Log("No PATH!");
                return null;
            }
            for (; step > -1; step --)
            {
                if (TestDirection(endX, endY, step, Directions.Up))
                    tempList.Add(CurrentGridArray[endX, endY + 1]);
                if (TestDirection(endX, endY, step, Directions.Right))
                    tempList.Add(CurrentGridArray[endX + 1, endY]);
                if (TestDirection(endX, endY, step, Directions.Down))
                    tempList.Add(CurrentGridArray[endX, endY - 1]);
                if (TestDirection(endX, endY, step, Directions.Left))
                    tempList.Add(CurrentGridArray[endX-1, endY]);

                GameObject tempObj = FindClosest(CurrentGridArray[endX, endY].transform, tempList);
                if(tempObj.TryGetComponent(out GridStatView gridStatView))
                {
                    path.Add(tempObj);
                    endX = tempObj.GetComponent<GridStatView>().x;
                    endY = tempObj.GetComponent<GridStatView>().y;
                    tempList.Clear();
                } 
            }
            foreach (var gameObject in _path)
            {
                var sr = gameObject.GetComponent<SpriteRenderer>();
                sr.color = new Color(0, 1, 1);
            }
            return path;
        }

        private void TestFourDirections(int x, int y, int step)
        {
            if(TestDirection(x, y, -1, Directions.Up))
                SetVisited(x, y + 1, step);
            if (TestDirection(x, y, -1, Directions.Right))
                SetVisited(x + 1, y, step);
            if (TestDirection(x, y, -1, Directions.Down))
                SetVisited(x, y - 1, step);
            if (TestDirection(x, y, -1, Directions.Left))
                SetVisited(x-1, y, step);
        }


        private bool TestDirection(int x, int y, int step, Directions directions)
        {
            int nearCell = 1;
            switch (directions)
            {
                case Directions.Up:
                    if (y + 1 < _gridControllerView.Rows && CurrentGridArray[x, y + nearCell] && CheckStep(x, y + nearCell, step))
                        return true;
                    else
                        return false;
                case Directions.Right:
                    if (x + 1 < _gridControllerView.Columns && CurrentGridArray[x + nearCell, y] && CheckStep(x + nearCell, y, step))
                        return true;
                    else
                        return false;
                case Directions.Down:
                    if (y - 1 > -1 && CurrentGridArray[x, y - nearCell] && CheckStep(x, y - nearCell, step))
                        return true;
                    else
                        return false;
                case Directions.Left:
                    if (x - 1 > -1 && CurrentGridArray[x - nearCell, y] && CheckStep(x - nearCell, y, step))
                        return true;
                    else
                        return false;
            }
            return false;
        }

        private bool CheckStep(int x, int y, int step)
        {
            CurrentGridArray[x, y].TryGetComponent<GridStatView>(out var gridStatView);
            if (gridStatView != null)
                return gridStatView.visited == step;
            else
                return false;
        }

        private void SetVisited(int x, int y, int step)
        {
            if (CurrentGridArray[x, y])
                CurrentGridArray[x, y].GetComponent<GridStatView>().visited = step;
        }

        private GameObject FindClosest(Transform targetLoacation, List<GameObject> list)
        {
            float currentDistance = _gridControllerView.Rows * _gridControllerView.Columns;
            int indexNumber = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if(Vector3.Distance(targetLoacation.position, list[i].transform.position) < currentDistance)
                {
                    currentDistance = Vector3.Distance(targetLoacation.position, list[i].transform.position);
                    indexNumber = i;
                }
            }
            return list[indexNumber];
        }


    }
}
