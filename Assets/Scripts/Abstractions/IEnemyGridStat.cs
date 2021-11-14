namespace PiggerBomber
{
    internal interface IEnemyGridStat
    {
        EnemiesType EnemyType { get; }
        int Visited { get; set; }
        int X { get; set; }
        int Y { get; set; }
    }
}
