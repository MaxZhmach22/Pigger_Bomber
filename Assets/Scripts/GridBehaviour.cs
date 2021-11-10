using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiggerBomber
{
    internal class GridBehaviour : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private int _rows = 9;
        [SerializeField] private int _columns = 17;
        public Vector3 _leftBorromLocation = new Vector3(0, 0, 0);
        [SerializeField] float _offsetX = 0.12f;

        [SerializeField] private GameObject _prefabTest;
        [SerializeField] private GameObject[,] _gridArray;

        [SerializeField] private int _startX = 0;
        [SerializeField] private int _startY = 0;
        [SerializeField] private int _endX = 5;
        [SerializeField] private int _endY = 5;

        public bool FindDistance = false;
        public List<GameObject> _path = new List<GameObject>();
        public float scaleX => _grid.cellSize.x;
        public float scaleY => _grid.cellSize.y;


        void Awake()
        {
            _gridArray = new GameObject[_columns, _rows];
            if (_prefabTest)
            {
                GenerateGrid();
            }
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
            for (int i = 0; i < _columns; i++)
            {
                float offset = 0;
                for (int j = 0; j < _rows; j++)
                {
                    GameObject obj = Instantiate(_prefabTest, new Vector3(
                        _leftBorromLocation.x + scaleX * i + offset,
                        _leftBorromLocation.y + j, _leftBorromLocation.z),
                        _grid.transform.rotation);
                    obj.transform.SetParent(gameObject.transform);
                    obj.GetComponent<GridStat>().x = i;
                    obj.GetComponent<GridStat>().y = j;
                    offset += _offsetX;
                    _gridArray[i, j] = obj;
                }
            }
        }

        /// <summary>
        /// В статрте указываем начальную точку игрока и выставляем занчение visited в -1, кроме позиции игрока.
        /// </summary>
        private void InitialSetup()
        {
            foreach (GameObject gameObject in _gridArray)
            {
                gameObject.GetComponent<GridStat>().visited = -1;
            }
            _gridArray[_startX, _startY].GetComponent<GridStat>().visited = 0;
        }

        private void SetDistance()
        {
            InitialSetup();
            int x = _startX;
            int y = _startY;
            int[] testArray = new int[_rows * _columns];
            for (int step = 1; step < _rows * _columns; step++)
            {
                foreach (GameObject gameObject in _gridArray) 
                {
                    if (gameObject && gameObject.GetComponent<GridStat>().visited == step - 1)
                        TestFourDirections(
                            gameObject.GetComponent<GridStat>().x,
                            gameObject.GetComponent<GridStat>().y,
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
            if(_gridArray[_endX, _endY] && _gridArray[_endX, _endY].GetComponent<GridStat>().visited > 0)
            {
                _path.Add(_gridArray[x, y]);
                step = _gridArray[x, y].GetComponent<GridStat>().visited - 1;
            }
            else
            {
                Debug.Log("No PATH!");
                return;
            }
            for (int i = step; step > -1; step --)
            {
                if (TestDirection(x, y, step, Directions.Up))
                    tempList.Add(_gridArray[x, y + 1]);
                if (TestDirection(x, y, step, Directions.Right))
                    tempList.Add(_gridArray[x + 1, y]);
                if (TestDirection(x, y, step, Directions.Down))
                    tempList.Add(_gridArray[x, y - 1]);
                if (TestDirection(x, y, step, Directions.Left))
                    tempList.Add(_gridArray[x-1, y]);

                GameObject tempObj = FindClosest(_gridArray[_endX, _endY].transform, tempList);
                _path.Add(tempObj);
                x = tempObj.GetComponent<GridStat>().x;
                y = tempObj.GetComponent<GridStat>().y;
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
                    if (y + 1 < _rows /*1) проверка на выход за рамки сетки*/ 
                        && _gridArray[x, y + 1] /*2) проверка на существование точки на сетке*/ 
                        && _gridArray[x, y + 1].GetComponent<GridStat>().visited == step /*3) проверка на соответсвие шагу*/)
                        return true;
                    else
                        return false;
                case Directions.Right:
                    if (x + 1 < _columns && _gridArray[x + 1, y] && _gridArray[x + 1, y].GetComponent<GridStat>().visited == step)
                        return true;
                    else
                        return false;
                case Directions.Down:
                    if (y - 1 > -1 && _gridArray[x, y - 1] && _gridArray[x, y - 1].GetComponent<GridStat>().visited == step)
                        return true;
                    else
                        return false;
                case Directions.Left:
                    if (x - 1 > -1 && _gridArray[x - 1, y] && _gridArray[x - 1, y].GetComponent<GridStat>().visited == step)
                        return true;
                    else
                        return false;
            }
            return false;
        }

        private void SetVisited(int x, int y, int step)
        {
            if (_gridArray[x, y])
                _gridArray[x, y].GetComponent<GridStat>().visited = step;
        }

        private GameObject FindClosest(Transform targetLoacation, List<GameObject> list)
        {
            float currentDistance = _rows * _columns;
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
