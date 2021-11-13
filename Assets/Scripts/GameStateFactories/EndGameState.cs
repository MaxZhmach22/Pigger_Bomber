using System;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class EndGameState : GameState
    {
        private LooseGameController _looseGameController;
        private readonly LooseGameController.Factory _looseGameControllerFactory;

        #region ClassLifeCycles

        public EndGameState(LooseGameController.Factory looseGameControllerFactory) =>
           _looseGameControllerFactory = looseGameControllerFactory;

        public override void Start()
        {
            Time.timeScale = 0f;
            _looseGameController = _looseGameControllerFactory.Create();
            _looseGameController.Start();
        }

        public override void Dispose() =>
            _looseGameController.Dispose(); 

        #endregion

        public override void Update() { }

        internal sealed class Factory : PlaceholderFactory<EndGameState>
        {
        }
    }
}

