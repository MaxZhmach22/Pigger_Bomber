using UnityEngine;
using Zenject;
using UniRx;
using System.Collections;

namespace PiggerBomber
{
    class PlayerMovementController : BaseController, ITickable
    {
        #region Fields

        private readonly ITreesViewController _treesViewController;
        private readonly DynamicJoystick _dynamicJoystick;
        private readonly Player _player;
        private readonly GridController _gridController;

        private Vector3 _origPosition;
        private Vector3 _targetPosition;
        private Vector2Int _startPosition = new Vector2Int(0, 4);
        private float _timeToMove = 0.5f;
        private float _treshHold = 0.5f;
        private bool _isMoving;

        #endregion


        #region ClassLifeCycles

        public PlayerMovementController(
            ITreesViewController treesViewController,
            DynamicJoystick dynamicJoystick,
            GridController gridController,
            Player player)
        {
            _treesViewController = treesViewController;
            _dynamicJoystick = dynamicJoystick;
            _gridController = gridController;
            _player = player;
        }

        public override void Start() =>
            SetStartPlayerPosition();

        public override void Dispose() =>
              Debug.Log($"{nameof(PlayerMovementController)} is Disposed");

        #endregion


        #region FraemWorkUpdateMethods

        public void Tick()
        {
            if (Mathf.Abs(_dynamicJoystick.Vertical) > _dynamicJoystick.Horizontal)
            {
                if (_dynamicJoystick.Vertical > _treshHold)
                    Move(Directions.Up);
                if (_dynamicJoystick.Vertical < -_treshHold)
                    Move(Directions.Down);
            }

            if (Mathf.Abs(_dynamicJoystick.Horizontal) > _dynamicJoystick.Vertical)
            {
                if (_dynamicJoystick.Horizontal > _treshHold)
                    Move(Directions.Right);
                if (_dynamicJoystick.Horizontal < -_treshHold)
                    Move(Directions.Left);
            }
        }

        private IEnumerator Move(Vector3 direction, Vector2Int newIndex)
        {
            float elapsedTime = 0;
            _origPosition = _player.transform.position;
            _targetPosition = direction;
            while (elapsedTime < _timeToMove)
            {
                _player.transform.position = Vector3.Lerp(_origPosition, _targetPosition, (elapsedTime / _timeToMove));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _player.transform.position = _targetPosition;
            _player.CurrentIndexInArray = newIndex;
            _isMoving = false;
        } 

        #endregion


        #region Methods

        private void Move(Directions direction)
        {
            switch (direction)
            {
                case Directions.Up:
                    if (CheckGridValue(Vector2Int.up) && !_isMoving)
                    {
                        _isMoving = true;
                        _player.SetSprite(Directions.Up);
                        MoveToNextPos(Vector2Int.up.x, Vector2Int.up.y);
                    }
                    break;
                case Directions.Right:
                    if (CheckGridValue(Vector2Int.right) && !_isMoving)
                    {
                        _isMoving = true;
                        _player.SetSprite(Directions.Right);
                        MoveToNextPos(Vector2Int.right.x, Vector2Int.right.y);
                    }
                    break;
                case Directions.Down:
                    if (CheckGridValue(Vector2Int.down) && !_isMoving)
                    {
                        _isMoving = true;
                        _player.SetSprite(Directions.Down);
                        MoveToNextPos(Vector2Int.down.x, Vector2Int.down.y);
                    }
                    break;
                case Directions.Left:
                    if (CheckGridValue(Vector2Int.left) && !_isMoving)
                    {
                        _isMoving = true;
                        _player.SetSprite(Directions.Left);
                        MoveToNextPos(Vector2Int.left.x, Vector2Int.left.y);
                    }
                    break;
            }
        }

        private void MoveToNextPos(int x, int y)
        {
            var key = new Vector2Int(_player.CurrentIndexInArray.x + x, _player.CurrentIndexInArray.y + y);
            _treesViewController.FreePositionInGrid.TryGetValue(key, out var gameObject);
            if (gameObject != null)
            {
                var nextPos = gameObject.transform.position;
                MainThreadDispatcher.StartUpdateMicroCoroutine(Move(nextPos, key));
            }

        }

        private bool CheckGridValue(Vector2Int direction)
        {
            if (_treesViewController.FreePositionInGrid.ContainsKey(new Vector2Int(_player.CurrentIndexInArray.x + direction.x, _player.CurrentIndexInArray.y + direction.y)))
                return true;
            else
                return false;
        }

        private void SetStartPlayerPosition()
        {
            _treesViewController.FreePositionInGrid.TryGetValue(_startPosition, out var gameObject);
            if (gameObject != null)
            {
                _player.transform.position = gameObject.transform.position;
                _player.CurrentIndexInArray = _startPosition;
            }
        }

        #endregion
    }
}
