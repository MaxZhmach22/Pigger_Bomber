using UnityEngine;

namespace PiggerBomber
{
    internal interface IGridController
    {
        GameObject[,] CurrentGridArray { get; }
    }
}
