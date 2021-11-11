using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class Bomb: MonoBehaviour, ITickable
    {
        public Subject<bool> IsExplosed = new Subject<bool>();

        [field: SerializeField] public float BombTimer { get; private set; }
        [field: SerializeField] public Sprite BombSprite { get; private set; }
        [field: SerializeField] public List<Sprite> FlameSprites { get; private set; }
        [field: SerializeField] public float SpeedFireAnimation { get; private set; }

        private float _timer;
        private float _exploseTimer;
        private SpriteRenderer  _spriteRenderer;
        private int _spriteIndex = 0;
        private Collider2D _collider;

        private void Start() 
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            _collider.enabled = true;
            gameObject.SetActive(false);
        }

        public void Tick()
        {
            if (gameObject.activeInHierarchy)
            {
                _timer += Time.deltaTime;
                _exploseTimer += Time.deltaTime;


                if(_timer >= SpeedFireAnimation)
                {
                    AnimateFire();
                    _timer = 0;
                }

                if (_exploseTimer >= BombTimer)
                    Explose();
            }
        }

        private void Explose()
        {
            _collider.enabled = true;
            IsExplosed.OnNext(true);
            _exploseTimer = 0;
            gameObject.SetActive(false);
        }

        private void AnimateFire()
        {
            if (_spriteIndex >= FlameSprites.Count)
                _spriteIndex = 0;

            _spriteRenderer.sprite = FlameSprites[_spriteIndex];
            _spriteIndex++;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            collision.gameObject.TryGetComponent<BaseEnemy>(out var enemy);
            if (enemy != null)
                enemy.GetDirty();
        }
    }
}