using UnityEngine;

namespace PiggerBomber
{
    internal abstract class GridStatBase : MonoBehaviour, IEnemyGridStat
    {
        public abstract EnemiesType EnemyType { get; protected set; }
        public int Visited { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
