using UnityEngine;

namespace PiggerBomber
{
    internal interface IGridController
    {
        GameObject[,] GridArray { get; }
    }
}
