using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

namespace PiggerBomber
{
    internal sealed class TreesViewController : BaseController, ITreesViewController
    {
        [Inject] private AppleTree _appleTree;
        [Inject] private DiContainer _diContainer;
        [Inject] private GridController _gridController;

        private readonly List<GameObject> _trees;
        private readonly Transform _parentTransform;
        public readonly List<Vector2Int> _matureTreesPositionList;
        private CompositeDisposable _disposables; 
        private int _countTreesInRow = 7;
        private int _upperBoundX; 
        private int _upperBoundY;

        public IReadOnlyList<Vector2Int> MatureTreesPositionList => _matureTreesPositionList;
        public GameObject[,] CurrentGridArray => _gridController.GridArray;

        public TreesViewController()
        {
            _trees = new List<GameObject>();
            _matureTreesPositionList = new List<Vector2Int>();
            _disposables = new CompositeDisposable();
            _parentTransform = new GameObject("Trees").transform;
        }

        public override void Start()
        {
            _upperBoundX = _gridController.GridArray.GetUpperBound(0);
            _upperBoundY = _gridController.GridArray.GetUpperBound(1);
            SetTreesOnMap();
            SubscribeOnTreesEvent();
        }

        public override void Dispose()
        {
            foreach (var tree in _trees)
            {
                _disposables.Clear();
                GameObject.Destroy(tree);
            }
            _trees.Clear();
            _matureTreesPositionList.Clear();
            GameObject.Destroy(_parentTransform);
        }

        private void SubscribeOnTreesEvent()
        {
            foreach (var tree in _trees)
            {
                tree.GetComponent<AppleTree>().MatureTreePosition.Subscribe(position => CheckMatureTrees(position)).AddTo(_disposables);
            }
        }

        private void CheckMatureTrees(Vector2Int position)
        {
            _matureTreesPositionList.Add(position);
            if (_matureTreesPositionList.Count > 4)
                _matureTreesPositionList.RemoveAt(0);
        }

        private void SetTreesOnMap()
        {
            for (int x = 0; x < _upperBoundX; x++)
            {
                for (int y = 0; y < _upperBoundY; y++)
                {
                    if (CheckValue(x, y, _upperBoundX, _upperBoundY))
                    {
                        var tree = _diContainer.InstantiatePrefab(_appleTree, _parentTransform);
                        tree.transform.position = _gridController.GridArray[x, y].transform.position;
                        tree.GetComponent<AppleTree>().SetTreeIndex(x, y);
                        var emptyGridObj = _gridController.GridArray[x, y];
                        GameObject.Destroy(emptyGridObj);
                        _gridController.GridArray[x, y] = tree.gameObject;
                        _trees.Add(tree);
                    }
                        
                }
            }
        }

        private bool CheckValue(int x, int y, int upperBoundX, int upperBoundY)
        {
            if (y == 0 || y  == upperBoundY)
                return false;
            if (x == 0 || x == _countTreesInRow + 1 || x == upperBoundX)
                return false;
            if (y % 2 == 0)
                return false;

            return true;
        }

    }
}
