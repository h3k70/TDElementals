using System;
using System.Collections.Generic;
using UnityEngine;

public class KillLogManagerUI : MonoBehaviour
{
    [SerializeField] private KillLogUI _killLogUIPref;

    private Queue<KillLogUI> _killLogs = new();
    private Base[] _bases = new Base[2];
    private List<Character> _characters = new List<Character>();

    public void Init(Base base1, Base base2)
    {
        _bases[0] = base1;
        _bases[1] = base2;

        base1.CharacterSpawned += CharacterSpawned;
        base2.CharacterSpawned += CharacterSpawned;
    }

    private void OnDestroy()
    {
        foreach (var c in _bases)
            c.CharacterSpawned -= CharacterSpawned;
    }

    private void CharacterSpawned(Character character)
    {
        character.Died += OnDied;
        _characters.Add(character);
    }

    private void OnDied(Damage damage)
    {
        AddLog(damage.DamageDealer, damage.Damageable);
        
        if (damage.Damageable is Character character)
        {
            _characters.Remove(character);
            character.Died -= OnDied;
        }
    }

    public void AddLog(IDamageDealer damageDealer, IDamageable damageable)
    {
        if (_killLogs.Count >= 3)
            Destroy(_killLogs.Dequeue().gameObject);

        var item = Instantiate(_killLogUIPref, transform);
        item.Init(damageDealer, damageable);
        _killLogs.Enqueue(item);
    }
}
