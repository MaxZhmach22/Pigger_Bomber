using ModestTree;
using UnityEngine;

namespace PiggerBomber
{
    internal sealed class GameStateFactory
    {
        readonly StartGameState.Factory _startStateFactory;
        readonly GameGameState.Factory _gameStateFactory;

        public GameStateFactory(
            StartGameState.Factory startStateFactory,
            GameGameState.Factory gameStateFactory)
        {
            _startStateFactory = startStateFactory;
            _gameStateFactory = gameStateFactory;
        }

        public GameState CreateState(GameStates state) 
        {
            switch (state)
            {
                case GameStates.Start:
                    return _startStateFactory.Create();
                case GameStates.Game:
                    return _gameStateFactory.Create();
                case GameStates.None:
                    break;
                case GameStates.End:
                    Debug.Log("End State");
                    break;
            }

            throw Assert.CreateException(); 
        }
    }
}
