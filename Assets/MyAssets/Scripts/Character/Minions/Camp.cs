using System;
using System.Collections.Generic;
using Mirror;
using UnityEditor.Rendering;
using UnityEngine;

public class Camp : NetworkBehaviour
{
    [SerializeField] private List<Minion> _minionsPrefs;

    private List<Minion> _minions = new();

    public List<Minion> Minions { get => _minions; }

    public event Action<Minion> MinionSpawned;

    public void Spawn()
    {
        if (isServer == false)
            return;

        var minion = Instantiate(_minionsPrefs[0], transform);
        NetworkServer.Spawn(minion.gameObject);
        _minions.Add(minion);

        MinionSpawned?.Invoke(minion);
    }
}
