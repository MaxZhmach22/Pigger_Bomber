﻿using System.Collections.Generic;
using UnityEngine;

namespace PiggerBomber
{
    internal sealed class DogEnemy : BaseEnemy
    {
        private SpriteRenderer _spriteRenderer;
        private SpriteStates _currentState;
        public bool IsDirty => _isDirty;
        private bool _isDirty;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _currentState = SpriteStates.Common;
        }

        public void SeePalyer()
        {
            _currentState = SpriteStates.Angry;
        }

        public override void GetDirty()
        {
            _isDirty = true;
            _currentState = SpriteStates.Dirty;
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

        protected override void SetSprites(Directions directions)
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
                    break;
                case Directions.Right:
                    _spriteRenderer.sprite = spritesList[1];
                    break;
                case Directions.Down:
                    _spriteRenderer.sprite = spritesList[2];
                    break;
                case Directions.Left:
                    _spriteRenderer.sprite = spritesList[3];
                    break;
            }
        }
    }
}
