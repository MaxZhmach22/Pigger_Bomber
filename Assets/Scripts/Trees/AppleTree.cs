using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

internal sealed class AppleTree : MonoBehaviour
{
    #region Fields

    public Subject<Vector2Int> MatureTreePosition = new Subject<Vector2Int>();

    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private float _minTimeValue;
    [SerializeField] private float _maxTimeValue;

    private SpriteRenderer _spriteRenderer;
    private float _timer;
    private float _rndTimeOfGrow;
    private int _spriteIndex;
    private int _matureTreeSprite = 7;
    private Vector2Int _indexInGridArray; 

    #endregion


    #region ClassLifeCycles

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        RandomizeTimer();
        _spriteIndex = UnityEngine.Random.Range(0, _sprites.Count - 1);
        _spriteRenderer.sprite = _sprites[_spriteIndex];
        MainThreadDispatcher.StartFixedUpdateMicroCoroutine(Grow());
        Grow().ToObservable().Subscribe().AddTo(this);
    }


    #endregion


    #region UniRxCoroutine

    private IEnumerator Grow()
    {
        if (!gameObject.activeInHierarchy)
            yield break;

        while (true)
        {
            _timer += Time.deltaTime;
            yield return null;

            if (_timer < _rndTimeOfGrow)
                continue;

            if (_spriteIndex >= _sprites.Count - 1)
            {
                _spriteIndex = 0;
                RandomizeTimer();
            }
            else
                _spriteIndex++;

            if (_spriteIndex == _matureTreeSprite)
            {
                SendPosition();
            }

            if (_spriteRenderer != null)
                _spriteRenderer.sprite = _sprites[_spriteIndex];
            _timer = 0;
        }
    }

    #endregion


    #region Methods

    public void SetTreeIndex(int x, int y)
    {
        _indexInGridArray.x = x;
        _indexInGridArray.y = y;
    }

    private void SendPosition() =>
        MatureTreePosition.OnNext(_indexInGridArray);

    private void RandomizeTimer() =>
        _rndTimeOfGrow = UnityEngine.Random.Range(_minTimeValue, _maxTimeValue); 

    #endregion

}
