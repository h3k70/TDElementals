using UnityEngine;

class Layers
{
    private const string _enemy = "Enemy";
    private const string _ally = "Ally";

    public static int Enemy => LayerMask.NameToLayer(_enemy);
    public static int EnemyMask => LayerMask.GetMask(_enemy);
    public static int Ally => LayerMask.NameToLayer(_ally);
    public static int AllyMask => LayerMask.GetMask(_ally);
}

