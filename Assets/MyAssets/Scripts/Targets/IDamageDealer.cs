using UnityEngine;

public interface IDamageDealer
{
    public Sprite Icon { get; }
    public Elements Element { get; }
    public GameObject Self { get; }

    public bool TryAttack(IDamageable target);
    public void DealDamage(float value, GameObject damageable);
}
