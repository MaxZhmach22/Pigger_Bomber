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

        protected abstract void SetSprites(Directions spriteStates);
        protected abstract void ChangeSpriteDirections(Directions directions, List<Sprite> spritesList);
        public abstract void GetDirty();


    }
}
