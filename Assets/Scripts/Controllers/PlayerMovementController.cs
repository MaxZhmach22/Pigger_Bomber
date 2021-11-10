using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace PiggyBomber
{
    class PlayerMovementController : BaseController, ITickable
    {
        private readonly DynamicJoystick _dynamicJoystick;
        private readonly Player _player;
        [Inject(Id = "Grid")] private Grid _grid;

        
        private float _playerStepSpeedTimer = 0.5f;
        private float _timer;

        public PlayerMovementController(
            DynamicJoystick dynamicJoystick,
           
            Player player)
        {
            _dynamicJoystick = dynamicJoystick;
            _player = player;
        }

        public override void Start()
        {
            Debug.Log(_grid.cellLayout);
            Vector3Int cellposition = _grid.WorldToCell(_grid.transform.position);
            Debug.Log(cellposition);
        }
        public override void Dispose()
        {
            Debug.Log($"{nameof(PlayerMovementController)} is Disposed");
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _playerStepSpeedTimer)
            {
                Move(_dynamicJoystick.Direction);
                Debug.Log($"X: {_dynamicJoystick.Direction.x} Y: {_dynamicJoystick.Direction.y}");
                _timer = 0;
            }
        }

        private void Move(Vector2 direction)
        {
            if(direction.x > 0 && direction.x> direction.y)
            {
                
                //_player.transform.position += new Vector3(_grid.cellSize.x, 0, 0);
                _player.transform.position += new Vector3(1, 0, 0);
            }
        }


    }
}
