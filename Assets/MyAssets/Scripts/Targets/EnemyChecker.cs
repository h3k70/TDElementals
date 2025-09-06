using System.Collections.Generic;
using UnityEngine;

public class EnemyChecker : MonoBehaviour
{
    private List<Character> _enemies = new();
    private List<Character> _enemiesOnPath = new();

    public List<Character> Enemies { get => _enemies; }

    public void Initialize(List<Character> enemiesOnPath)
    {
        _enemiesOnPath = enemiesOnPath;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character enemy) && _enemiesOnPath.Contains(enemy))
        {
            _enemies.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Character enemy))
        {
            _enemies.Remove(enemy);
        }
    }
}
