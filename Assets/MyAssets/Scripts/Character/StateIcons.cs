using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StateIcons", menuName = "StateIcons")]
class StateIcons : ScriptableObject
{
    [SerializeField] private List<States> _buffs = new();
    [SerializeField] private List<Sprite> _sprite = new();

    private Dictionary<States, Sprite> _stateIcons = new();

    public Dictionary<States, Sprite> Icons { get => _stateIcons; }

    private void OnValidate()
    {
        if (_buffs.Count != _sprite.Count)
            return;

        _stateIcons.Clear();
        for (int i = 0; i < _buffs.Count; i++)
        {
            _stateIcons.Add(_buffs[i], _sprite[i]);
        }
    }
}