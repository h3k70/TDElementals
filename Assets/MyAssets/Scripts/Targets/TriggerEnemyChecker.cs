using System.Collections.Generic;
using UnityEngine;

public class TriggerEnemyChecker : MonoBehaviour, IEnemyChecker
{
    private List<ITargetable> _enemies = new();

    public List<ITargetable> Enemies { get => _enemies; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ITargetable enemy))
        {
            _enemies.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ITargetable enemy))
        {
            _enemies.Remove(enemy);
        }
    }
}
