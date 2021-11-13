using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class Bomb: MonoBehaviour
    {
        #region Fields

        [Inject] private Player _player;
        [field: SerializeField] public float BombTimer { get; private set; }
        [field: SerializeField] public Sprite BombSprite { get; private set; }
        [field: SerializeField] public List<Sprite> FlameSprites { get; private set; }
        [field: SerializeField] public float SpeedFireAnimation { get; private set; }
        [field: SerializeField] public float ExplosionRadius { get; private set; }

        private int _spriteIndex = 0;
        private bool _isExploed = false;
        private SpriteRenderer _flameSpriteRendere;

        #endregion


        #region ClassLifeCycles

        private void Awake()
        {
            _flameSpriteRendere = GetComponent<SpriteRenderer>();
            gameObject.SetActive(false);
        }

        #endregion


        #region Methods

        public void OnExplosed()
        {
            _isExploed = true;
            gameObject.SetActive(false);
        }
            

        public void AnimateFire()
        {
            if (_spriteIndex >= FlameSprites.Count)
                _spriteIndex = 0;

            _flameSpriteRendere.sprite = FlameSprites[_spriteIndex];
            _spriteIndex++;
        } 

        #endregion
    }
}