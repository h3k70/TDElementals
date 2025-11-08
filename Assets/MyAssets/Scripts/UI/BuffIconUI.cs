using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class BuffIconUI : MonoBehaviour
{
    [SerializeField] private BuffIcons _buffIcons;
    [SerializeField] private Image _icon;

    private Attributes _buff;

    public Attributes Buff { get => _buff; }

    public void Init(Attributes buff)
    {
        _buff = buff;
        _icon.sprite = _buffIcons.Icons.GetValueOrDefault(buff);
    }
}