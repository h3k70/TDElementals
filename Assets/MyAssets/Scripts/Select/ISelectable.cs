using System;
using UnityEngine;

public interface ISelectable
{
    public GameObject Self { get; }
    public Sprite Icon { get; }
    public bool IsSelected { get; }

    public event Action<ISelectable> Selected;
    public event Action<ISelectable> Deselected;

    public void Select();
    public void Deselect();
}
