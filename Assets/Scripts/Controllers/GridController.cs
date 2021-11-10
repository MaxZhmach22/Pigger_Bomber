using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace PiggerBomber
{
    internal sealed class GridController : BaseController, IGridController
    {
        private readonly GridControllerView _gridControllerView;

        private int _startX = 0;
        private int _startY = 0;
        private int _endX = 5;
        private int _endY = 5;

        public bool FindDistance = false;

        private Transform _parentTransform;
        private Vector3 _leftBorromLocation = new Vector3(0, 0, 0);
        private float _scaleX => _gridControllerView.Grid.cellSize.x;
        private float _scaleY => _gridControllerView.Grid.cellSize.y;
        public GameObject[,] GridArray { get; private set; }
        public IReadOnlyList<GameObject> Path => _path;
        
        private List<GameObject> _path = new List<GameObject>();

        public GridController(GridControllerView gridControllerView)
        {
            _gridControllerView = gridControllerView;
        } 

        public override void Start()
        {
            GridArray = new GameObject[_gridControllerView.Columns, _gridControllerView.Rows];
            _parentTransform = new GameObject("GridArray").transform;
            if (_gridControllerView.GridPointPrefab)
                GenerateGrid();
        }
        public override void Dispose()
        {
           foreach(GameObject gameObject in GridArray)
                GameObject.Destroy(gameObject);
            Array.Clear(GridArray, 0, GridArray.Length);

            _path.Clear();
            GameObject.Destroy(_parentTransform);
        }


        // Update is called once per frame
        void Update()
        {
            if (FindDistance)
            {
                SetDistance();
                SetPath();
                FindDistance = false;
            }
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
                    GridArray[i, j] = obj;
                }
            }
        }

        /// <summary>
        /// В статрте указываем начальную точку игрока и выставляем занчение visited в -1, кроме позиции игрока.
        /// </summary>
        private void InitialSetup()
        {
            foreach (GameObject gameObject in GridArray)
            {
                gameObject.GetComponent<GridStatView>().visited = -1;
            }
            GridArray[_startX, _startY].GetComponent<GridStatView>().visited = 0;
        }

        private void SetDistance()
        {
            InitialSetup();
            int x = _startX;
            int y = _startY;
            int[] testArray = new int[_gridControllerView.Rows * _gridControllerView.Columns];
            for (int step = 1; step < _gridControllerView.Rows * _gridControllerView.Columns; step++)
            {
                foreach (GameObject gameObject in GridArray) 
                {
                    if (gameObject && gameObject.GetComponent<GridStatView>().visited == step - 1)
                        TestFourDirections(
                            gameObject.GetComponent<GridStatView>().x,
                            gameObject.GetComponent<GridStatView>().y,
                            step);
                }
            }
        }

        private void SetPath()
        {
            int step;
            int x = _endX;
            int y = _endY;
            List<GameObject> tempList = new List<GameObject>();
            _path.Clear();
            if(GridArray[_endX, _endY] && GridArray[_endX, _endY].GetComponent<GridStatView>().visited > 0)
            {
                _path.Add(GridArray[x, y]);
                step = GridArray[x, y].GetComponent<GridStatView>().visited - 1;
            }
            else
            {
                Debug.Log("No PATH!");
                return;
            }
            for (int i = step; step > -1; step --)
            {
                if (TestDirection(x, y, step, Directions.Up))
                    tempList.Add(GridArray[x, y + 1]);
                if (TestDirection(x, y, step, Directions.Right))
                    tempList.Add(GridArray[x + 1, y]);
                if (TestDirection(x, y, step, Directions.Down))
                    tempList.Add(GridArray[x, y - 1]);
                if (TestDirection(x, y, step, Directions.Left))
                    tempList.Add(GridArray[x-1, y]);

                GameObject tempObj = FindClosest(GridArray[_endX, _endY].transform, tempList);
                _path.Add(tempObj);
                x = tempObj.GetComponent<GridStatView>().x;
                y = tempObj.GetComponent<GridStatView>().y;
                tempList.Clear();
            }
            foreach (var gameObject in _path)
            {
                var sr = gameObject.GetComponent<SpriteRenderer>();
                sr.color = new Color(0, 1, 1);
            }
           
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
            switch (directions)
            {
                case Directions.Up:
                    if (y + 1 < _gridControllerView.Rows /*1) проверка на выход за рамки сетки*/
                        && GridArray[x, y + 1] /*2) проверка на существование точки на сетке*/ 
                        && GridArray[x, y + 1].GetComponent<GridStatView>().visited == step /*3) проверка на соответсвие шагу*/)
                        return true;
                    else
                        return false;
                case Directions.Right:
                    if (x + 1 < _gridControllerView.Columns && GridArray[x + 1, y] && GridArray[x + 1, y].GetComponent<GridStatView>().visited == step)
                        return true;
                    else
                        return false;
                case Directions.Down:
                    if (y - 1 > -1 && GridArray[x, y - 1] && GridArray[x, y - 1].GetComponent<GridStatView>().visited == step)
                        return true;
                    else
                        return false;
                case Directions.Left:
                    if (x - 1 > -1 && GridArray[x - 1, y] && GridArray[x - 1, y].GetComponent<GridStatView>().visited == step)
                        return true;
                    else
                        return false;
            }
            return false;
        }

        private void SetVisited(int x, int y, int step)
        {
            if (GridArray[x, y])
                GridArray[x, y].GetComponent<GridStatView>().visited = step;
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
