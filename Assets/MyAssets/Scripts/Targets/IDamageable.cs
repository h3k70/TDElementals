using System;
using UnityEngine;

public interface IDamageable
{
    public Sprite Icon { get; }
    public GameObject Self { get; }
    public float TempDamageValue {  get; set; }
    public bool IsCanTakeDamage { get; }
    public Elements Element { get; }

    public event Action<IDamageable> BeforDamageTaked;
    public event Action<IDamageable, float> DamageTaked;

    public void TakeDamage(Damage damage);
}
