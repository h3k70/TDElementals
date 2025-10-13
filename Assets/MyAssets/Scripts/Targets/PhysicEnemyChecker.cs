using System.Collections.Generic;

public class PhysicEnemyChecker : IEnemyChecker
{
    private List<Character> _enemies = new();
    private List<Character> _enemiesOnPath = new();

    public List<Character> Enemies { get => _enemies; }

    public PhysicEnemyChecker(List<Character> enemiesOnPath)
    {
        _enemiesOnPath = enemiesOnPath;
    }

    public void Update()
    {
        
    }
}
