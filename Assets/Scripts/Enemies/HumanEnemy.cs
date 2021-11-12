using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class HumanEnemy : BaseEnemy
    {
        private Player _player;
        private bool _isDirty;
        public bool IsDirty => _isDirty;
        public override int PathIndex { get; set; }
        public override float CurrentSpeed => _currentSpeed;
        public override List<GameObject> Path { get; set; }
        public override bool IsMoving { get; set; }

        public Subject<bool> SeePlayer = new Subject<bool>();

        [Inject]
        private void Init(Player player)
        {
            _player = player;
        }

        private void Start()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _currentState = SpriteStates.Common;
            _currentDirection = Directions.Left;
            _currentSpeed = CommonWalkingSpeed;
            _collider = GetComponent<Collider2D>();
            Path = new List<GameObject>();
            IsMoving = false;
        }

        public float SetSpeed()
        {
            switch (_currentState)
            {
                case SpriteStates.Common:
                    return CommonWalkingSpeed;
                case SpriteStates.Angry:
                    return AngryWalkingSpeed;
                case SpriteStates.Dirty:
                    return DirtyWalkingSpeed;
            }
            return CommonWalkingSpeed;
        }

        public override void GetDirty()
        {
            _isDirty = true;
            _collider.enabled = true;
            _currentState = SpriteStates.Dirty;
            _currentSpeed = SetSpeed(); 
            SetSprites(_currentDirection);
        }

        public override void SetSprites(Directions directions)
        {
            switch (_currentState)
            {
                case SpriteStates.Common:
                    ChangeSpriteDirections(directions, CommonSpites);
                    break;
                case SpriteStates.Angry:
                    ChangeSpriteDirections(directions, AngrySprites);
                    break;
                case SpriteStates.Dirty:
                    ChangeSpriteDirections(directions, DirtySprites);
                    break;
            }
        }

        protected override void ChangeSpriteDirections(Directions directions, List<Sprite> spritesList)
        {
            if (spritesList.Count < 4)
                return;

            switch (directions)
            {
                case Directions.Up:
                    _spriteRenderer.sprite = spritesList[0];
                    _currentDirection = Directions.Up;
                    break;
                case Directions.Right:
                    _spriteRenderer.sprite = spritesList[1];
                    _currentDirection = Directions.Right;
                    break;
                case Directions.Down:
                    _spriteRenderer.sprite = spritesList[2];
                    _currentDirection = Directions.Down;
                    break;
                case Directions.Left:
                    _spriteRenderer.sprite = spritesList[3];
                    _currentDirection = Directions.Left;
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _currentState = SpriteStates.Angry;
                _currentSpeed = SetSpeed();
                SetSprites(_currentDirection);
                SeePlayer.OnNext(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _currentState = SpriteStates.Common;
                _currentSpeed = SetSpeed();
                SetSprites(_currentDirection);
                SeePlayer.OnNext(false);
            }
        }
    }
}