using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffIcons", menuName = "BuffIcons")]
class BuffIcons : ScriptableObject
{
    [SerializeField] private List<Buffs> _buffs = new();
    [SerializeField] private List<Sprite> _sprite = new();

    private Dictionary<Buffs, Sprite> _buffIcons = new();

    public Dictionary<Buffs, Sprite> Icons { get => _buffIcons; }

    private void OnValidate()
    {
        if (_buffs.Count != _sprite.Count)
            return;

        _buffIcons.Clear();
        for (int i = 0; i < _buffs.Count; i++)
        {
            _buffIcons.Add(_buffs[i], _sprite[i]);
        }
    }
}
