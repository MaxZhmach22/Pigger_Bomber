using System;
using UniRx;

namespace PiggerBomber
{
    internal interface IPlantBomb
    {
        ISubject<bool> BombIsPLanted { get; }
        void PlantBomb(Bomb bomb);
    }
}