using System;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    class PlayerMovementController : BaseController, ITickable
    {
        private readonly IGridController _gridController;
        private readonly DynamicJoystick _dynamicJoystick;
        private readonly Player _player;

        
        private float _playerStepSpeedTimer = 0.5f;
        private float _timer;

        public PlayerMovementController(IGridController gridController,
            DynamicJoystick dynamicJoystick,
           
            Player player)
        {
            _gridController = gridController;
            _dynamicJoystick = dynamicJoystick;
            _player = player;
        }

        public override void Start()
        {
          
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
