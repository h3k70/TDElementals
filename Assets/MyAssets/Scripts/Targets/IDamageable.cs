using System;
using UnityEngine;

public interface IDamageable
{
    public Sprite Icon { get; }
    public Damage TempDamage {  get; set; }

    public Action<IDamageable> BeforDamageTaked { get; set; }
    public Action<IDamageable> DamageTaked { get; set; }

    public void TakeDamage(float damage);
}
