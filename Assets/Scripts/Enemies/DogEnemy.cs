using System.Collections.Generic;
using UnityEngine;

namespace PiggerBomber
{
    internal sealed class DogEnemy : BaseEnemy
    {
        #region Fields

        private IEnemiesMovingController _enemiesMovingController;
        public override bool IsMoving { get; set; }
        public bool IsDirty { get; set; }
        public override int PathIndex { get; set; }
        public override float CurrentSpeed => _currentSpeed;
        public override EnemiesType EnemiesType => EnemiesType.Dog;
        public override List<GameObject> Path { get; set; } 

        #endregion


        #region ClassLifeCycles

        private void Start()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _currentState = SpriteStates.Common;
            _currentDirection = Directions.Left;
            _currentSpeed = CommonWalkingSpeed;
            _collider = GetComponent<Collider2D>();
            Path = new List<GameObject>();
            IsMoving = false;
            gameObject.SetActive(false);
        }
        public void Init(IEnemiesMovingController enemiesMovingController) =>
            _enemiesMovingController = enemiesMovingController;

        #endregion

        #region Methods

        public override void ResetAllValues()
        {
            _currentDirection = Directions.Left;
            _currentSpeed = CommonWalkingSpeed;
            _currentState = SpriteStates.Common;
            _collider.enabled = true;
            IsDirty = false;
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
            _collider.enabled = false;
            _currentState = SpriteStates.Dirty;
            _currentSpeed = SetSpeed();
            IsDirty = true;
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
            if (collision.gameObject.CompareTag("Player") && !IsDirty)
            {
                _currentState = SpriteStates.Angry;
                _currentSpeed = SetSpeed();
                SetSprites(_currentDirection);
                _enemiesMovingController.SetDogPath(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && !IsDirty)
            {
                _currentState = SpriteStates.Common;
                _currentSpeed = SetSpeed();
                SetSprites(_currentDirection);
                _enemiesMovingController.SetDogPath(false);
            }
        } 
        #endregion


    }
}
