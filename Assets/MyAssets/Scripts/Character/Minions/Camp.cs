using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Camp : NetworkBehaviour
{
    [SerializeField] private List<Minion> _minionsPrefs;

    private List<Minion> _minions = new();
    private Coroutine _spawnCoroutine;

    public List<Minion> Minions { get => _minions; }

    public event Action<Minion> MinionSpawned;

    public void Spawn()
    {
        if (_spawnCoroutine == null)
            _spawnCoroutine = StartCoroutine(SpawnJob());
    }

    private IEnumerator SpawnJob()
    {
        while (true)
        {
            yield return null;

            _minions.RemoveAll(item => item == null);

            
            if (_minions.Count <= 0)
            {
                var minion = Instantiate(_minionsPrefs[0], transform);
                NetworkServer.Spawn(minion.gameObject);
                _minions.Add(minion);

                MinionSpawned?.Invoke(minion);
            }
            Debug.Log(_minions[0]);
        }
    }
}
