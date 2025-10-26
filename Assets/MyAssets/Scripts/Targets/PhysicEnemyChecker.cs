using System.Collections.Generic;

public class PhysicEnemyChecker : IEnemyChecker
{
    private List<ITargetable> _enemies = new();
    private List<ITargetable> _enemiesOnPath = new();

    public List<ITargetable> Enemies { get => _enemies; }

    public PhysicEnemyChecker(List<ITargetable> enemiesOnPath)
    {
        _enemiesOnPath = enemiesOnPath;
    }

    public void Update()
    {
        
    }
}
