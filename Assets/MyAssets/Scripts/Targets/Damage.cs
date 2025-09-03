using UnityEngine;

public struct Damage
{
    public float Value;
    public IDamageable Target;
    public IDamageDealer DamageDealer;
}