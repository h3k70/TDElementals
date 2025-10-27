using System;
using Mirror;
using UnityEngine;

public abstract class UnitWithHP : NetworkBehaviour, IDamageable
{
    [SyncVar] private bool _isDead;
    [SyncVar, SerializeField] private float _health;
    [SerializeField] private Sprite _icon;
    [SerializeField] private Elements _element;

    public float TempDamageValue { get; set; }
    public float Health { get { return _health; } private set { HPChanged?.Invoke(Health, value); RpcHPChanged(Health, value); _health = value; } }

    public Sprite Icon => _icon;

    public GameObject Self => gameObject;

    public bool IsCanTakeDamage => !_isDead;

    public Elements Element => throw new NotImplementedException();

    public event Action<Damage> BeforDamageTaked;
    public event Action<Damage> DamageTaked;
    public event Action<Damage> Died;
    public event Action<float, float> HPChanged;

    public void TakeDamage(Damage damage)
    {
        TempDamageValue = damage.Value;
        BeforDamageTaked?.Invoke(damage);

        if (TempDamageValue > 0)
        {
            Health -= TempDamageValue;

            if (Health < 0)
            {
                _isDead = true;
                Health = 0;
                Died?.Invoke(damage);
            }
            DamageTaked?.Invoke(damage);
            RpcDamageTaked(damage.Value, damage.DamageDealer.Self, damage.Damageable.Self);
        }
    }

    [ClientRpc]
    private void RpcDamageTaked(float value, GameObject damageDealer, GameObject damageable)
    {
        Damage damage = new(value, damageDealer.GetComponent<IDamageDealer>(), damageable.GetComponent<IDamageable>());
        DamageTaked?.Invoke(damage);
    }

    [ClientRpc]
    private void RpcHPChanged(float oldValue, float newValue)
    {
        HPChanged?.Invoke(oldValue, newValue);
    }
}
