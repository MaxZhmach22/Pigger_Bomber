using System.Collections.Generic;
using UnityEngine;

namespace PiggerBomber
{
    internal interface ITreesViewController
    {
         IReadOnlyList<Vector2Int> MatureTreesPositionList { get; }
         IReadOnlyDictionary<Vector2Int, GameObject> FreePositionInGrid { get; }

    }
}