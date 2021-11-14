using System.Collections.Generic;
using UnityEngine;

namespace PiggerBomber
{
    internal abstract class BaseEnemy : MonoBehaviour
    {

        [field: Header("Sprites settings:")]
        [field: SerializeField] public List<Sprite> CommonSpites { get; private set; }
        [field: SerializeField] public List<Sprite> AngrySprites { get; private set; }
        [field: SerializeField] public List<Sprite> DirtySprites { get; private set; }

        [field: Header("Speed settings:")]
        [field: SerializeField] public float CommonWalkingSpeed { get; private set; }
        [field: SerializeField] public float AngryWalkingSpeed { get; private set; }
        [field: SerializeField] public float DirtyWalkingSpeed { get; private set; }

        protected float _currentSpeed;
        protected SpriteRenderer _spriteRenderer;
        protected SpriteStates _currentState;
        protected Collider2D _collider;
        protected Directions _currentDirection;

        public abstract EnemiesType EnemiesType { get; }
        public abstract bool IsMoving { get; set; }
        public abstract List<GameObject> Path { get; set; }
        public abstract int PathIndex { get; set; }
        public abstract float CurrentSpeed { get; }
        public abstract void SetSprites(Directions spriteStates);
        protected abstract void ChangeSpriteDirections(Directions directions, List<Sprite> spritesList);
        public abstract void GetDirty();
        public abstract void ResetAllValues();
    }
}
