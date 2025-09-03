using System;
using UnityEngine;

public class Character : MonoBehaviour, ISelectable
{
    private Sprite _icon;

    public Sprite Icon => _icon;

    public bool IsSelected { get; private set; }

    public event Action<ISelectable> Selected;
    public event Action<ISelectable> Deselected;

    public void Deselect()
    {
        Deselected?.Invoke(this);
    }

    public void Select()
    {
        Selected?.Invoke(this);
    }
}
