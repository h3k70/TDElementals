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

    public event Action<IDamageable> BeforDamageTaked;
    public event Action<IDamageable, float> DamageTaked;
    public event Action<Damage> Died;
    public event Action<float, float> HPChanged;

    public void TakeDamage(Damage damage)
    {
        TempDamageValue = damage.Value;
        BeforDamageTaked?.Invoke(this);

        if (TempDamageValue > 0)
        {
            Health -= TempDamageValue;

            if (Health < 0)
            {
                _isDead = true;
                Health = 0;
                Died?.Invoke(damage);
            }
            DamageTaked?.Invoke(this, damage.Value);
            RpcDamageTaked(damage.Value);
        }
    }

    [ClientRpc]
    private void RpcDamageTaked(float damage)
    {
        DamageTaked?.Invoke(this, damage);
    }

    [ClientRpc]
    private void RpcHPChanged(float oldValue, float newValue)
    {
        HPChanged?.Invoke(oldValue, newValue);
    }
}
