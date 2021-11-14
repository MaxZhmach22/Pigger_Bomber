using UnityEngine;

namespace PiggerBomber
{
    internal sealed class HumanGridStat : GridStatBase
    {
        #region Fields

        [field: SerializeField] public override EnemiesType EnemyType { get; protected set; }
     
        #endregion
    }
}
