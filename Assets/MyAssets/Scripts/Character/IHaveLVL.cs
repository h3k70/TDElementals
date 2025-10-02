using System;
using UnityEngine;

public interface IHaveLVL
{
    public int CurrentLVL { get; }
    public int MaxLVL { get; }
    public float CurrentExp { get; }
    public float ExpForNextLVL { get; }

    public event Action<int, int> LVLChanged;
    public event Action<float, float> ExpChanged;

    public void SetLVL(int lvl);
    public void AddExp(float value);
}
