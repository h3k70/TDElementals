using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class BuffIconUI : MonoBehaviour
{
    [SerializeField] private BuffIcons _buffIcons;
    [SerializeField] private Image _icon;

    private Buffs _buff;

    public Buffs Buff { get => _buff; }

    public void Init(Buffs buff)
    {
        _buff = buff;
        _icon.sprite = _buffIcons.Icons.GetValueOrDefault(buff);
    }
}