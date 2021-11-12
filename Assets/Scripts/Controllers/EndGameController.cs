using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class EndGameController : BaseController
    {
        private readonly LooseGameController _looseGameController;

        public EndGameController(
             LooseGameController looseGameController)
        {
            _looseGameController = looseGameController;
        }

        public override void Start()
        {
            _looseGameController.Start();
        }

        public override void Dispose()
        {
            _looseGameController?.Dispose();
            Debug.Log(nameof(MainGameController) + " Disposed");
        }

        public sealed class Factory : PlaceholderFactory<EndGameController>
        {
        }
    }
}

