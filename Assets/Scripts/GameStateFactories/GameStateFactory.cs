using ModestTree;
using UnityEngine;

namespace PiggerBomber
{
    internal sealed class GameStateFactory
    {
        #region Fields

        readonly StartGameState.Factory _startStateFactory;
        readonly GameGameState.Factory _gameStateFactory;
        readonly EndGameState.Factory _endStateFactory; 

        #endregion

        #region ClassLifeCycles

        public GameStateFactory(
          StartGameState.Factory startStateFactory,
          GameGameState.Factory gameStateFactory,
          EndGameState.Factory endStateFactory)
        {
            _startStateFactory = startStateFactory;
            _gameStateFactory = gameStateFactory;
            _endStateFactory = endStateFactory;
        } 

        #endregion


        public GameState CreateState(GameStates state) 
        {
            switch (state)
            {
                case GameStates.Start:
                    return _startStateFactory.Create();
                case GameStates.Game:
                    return _gameStateFactory.Create();
                case GameStates.End:
                    return _endStateFactory.Create();
                case GameStates.None:
                    break;
            }
            throw Assert.CreateException(); 
        }
    }
}
