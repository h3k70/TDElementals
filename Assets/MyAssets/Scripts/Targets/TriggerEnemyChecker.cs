using System.Collections.Generic;
using UnityEngine;

public class TriggerEnemyChecker : MonoBehaviour, IEnemyChecker
{
    private List<Character> _enemies = new();

    public List<Character> Enemies { get => _enemies; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character enemy))
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
