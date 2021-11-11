using UnityEngine;
using Zenject;
using UniRx;
using System;
using System.Collections.Generic;

namespace PiggerBomber
{
    internal class Player : MonoBehaviour, IPlantBomb
    {
        [field: SerializeField] public List<Sprite> DirectionsSprites { get; private set; }
        
        public Subject<Collision> CollisionGameObject = new Subject<Collision>();

        private GameState _state;
        private GameStateFactory _gameStateFactory;
        private SpriteRenderer _spriteRenderer;

        public GameStates CurrentGameState { get; private set; }
        public Vector2Int CurrentIndexInArray { get; set; }

        [Inject]
        public void Init(GameStateFactory gameStateFactory)
        {
            _gameStateFactory = gameStateFactory;
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Start()
        {
            ChangeState(GameStates.Game);
            gameObject.SetActive(false);
        }
           
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

        private void OnCollisionEnter(Collision collision) =>
            CollisionGameObject.OnNext(collision);

        public void PlanBomb(Bomb bomb)
        {
            bomb.gameObject.transform.position = transform.position;
            bomb.gameObject.SetActive(true);
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
    }
}