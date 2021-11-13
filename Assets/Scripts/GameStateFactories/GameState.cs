using System;
using UnityEngine;

namespace PiggerBomber
{
    internal abstract class GameState : IDisposable
    {
        public abstract void Update();

        public virtual void Start() { }

        public virtual void Dispose() { }

        public virtual void OnTriggerEnter(Collider other) { }

    }
}