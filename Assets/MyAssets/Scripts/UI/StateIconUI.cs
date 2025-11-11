using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateIconUI : MonoBehaviour
{
    [SerializeField] private StateIcons _buffIcons;
    [SerializeField] private Image _icon;

    private States _buff;

    public States Buff { get => _buff; }

    public void Init(States buff)
    {
        _buff = buff;
        _icon.sprite = _buffIcons.Icons.GetValueOrDefault(buff);
    }
}
