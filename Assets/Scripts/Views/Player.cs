using UnityEngine;
using Zenject;
using UniRx;
using System.Collections.Generic;

namespace PiggerBomber
{
    internal class Player : MonoBehaviour, IPlantBomb
    {
        #region Fields

        [Inject] Bomb _bomb;
        [field: SerializeField] public List<Sprite> DirectionsSprites { get; private set; }
        [field: SerializeField] public float PlayerMovementSpeed { get; private set; }


        private GameState _state;
        private GameStateFactory _gameStateFactory;
        private SpriteRenderer _spriteRenderer;
        private Subject<bool> _bombIsPlanted = new Subject<bool>();

        public bool IsCatched { get; private set; }
        public GameStates CurrentGameState { get; private set; }
        public Vector2Int CurrentIndexInArray { get; set; }

        public ISubject<bool> BombIsPLanted => _bombIsPlanted;
        public Subject<Collision> CollisionGameObject = new Subject<Collision>();

        #endregion


        #region ClassLifeCycles

        [Inject]
        public void Init(GameStateFactory gameStateFactory)
        {
            _gameStateFactory = gameStateFactory;
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Start() =>
            ChangeState(GameStates.Start);


        #endregion


        #region Methods

        public void ChangeState(GameStates state)
        {
            if (_state != null)
            {
                _state.Dispose();
                _state = null;
            }
            CurrentGameState = state;
            _state = _gameStateFactory.CreateState(state);
            _state.Start();
        }

        public void PlantBomb()
        {
            _bomb.gameObject.SetActive(true);
            _bomb.gameObject.transform.position = transform.position;
            _bombIsPlanted?.OnNext(true);
        }

        internal void SetSprite(Directions directions)
        {
            if (DirectionsSprites.Count < 4)
                return;

            switch (directions)
            {
                case Directions.Up:
                    _spriteRenderer.sprite = DirectionsSprites[0];
                    break;
                case Directions.Right:
                    _spriteRenderer.sprite = DirectionsSprites[1];
                    break;
                case Directions.Down:
                    _spriteRenderer.sprite = DirectionsSprites[2];
                    break;
                case Directions.Left:
                    _spriteRenderer.sprite = DirectionsSprites[3];
                    break;
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<BaseEnemy>(out var enemy))
                IsCatched = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent<BaseEnemy>(out var enemy))
                IsCatched = false;
        } 
        #endregion
    }
}