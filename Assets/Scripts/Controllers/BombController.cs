using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiggerBomber
{
    internal sealed class BombController : BaseController
    {
        private readonly Bomb _bomb;
        private readonly IPlantBomb _bombSetter;

        #region ClassLifeCycles

        public BombController(Bomb bomb, IPlantBomb bombSetter)
        {
            _bomb = bomb;
            _bombSetter = bombSetter;
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
