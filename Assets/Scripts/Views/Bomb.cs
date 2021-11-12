using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class Bomb: MonoBehaviour, ILateTickable
    {
        #region Fields

        [field: SerializeField] public float BombTimer { get; private set; }
        [field: SerializeField] public Sprite BombSprite { get; private set; }
        [field: SerializeField] public List<Sprite> FlameSprites { get; private set; }
        [field: SerializeField] public float SpeedFireAnimation { get; private set; }

        private int _spriteIndex = 0;
        private bool _isExploed = false;
        private SpriteRenderer _spriteRenderer;

        #endregion


        #region ClassLifeCycles

        private void Start()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            gameObject.SetActive(false);
        }

        #endregion


        #region ZenjectUpdateMethods

        public void LateTick()
        {
            if (_isExploed)
            {
                gameObject.SetActive(false);
                _isExploed = false;
            }
        } 
        private void OnTriggerStay2D(Collider2D collision)
        {
            collision.gameObject.TryGetComponent<BaseEnemy>(out var enemy);
            if (enemy != null && _isExploed)
                enemy.GetDirty();
        }

        #endregion


        #region Methods

        public void OnExplosed() =>
            _isExploed = true;

        public void AnimateFire()
        {
            if (_spriteIndex >= FlameSprites.Count)
                _spriteIndex = 0;

            _spriteRenderer.sprite = FlameSprites[_spriteIndex];
            _spriteIndex++;
        } 

        #endregion

    }
}