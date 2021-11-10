using System.Collections.Generic;
using UnityEngine;

namespace PiggerBomber
{
    internal interface ITreesViewController
    {
         IReadOnlyList<Vector2Int> MatureTreesPositionList { get; }
         GameObject[,] CurrentGridArray { get; }
    }
}