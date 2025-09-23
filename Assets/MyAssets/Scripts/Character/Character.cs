using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Character : NetworkBehaviour, ISelectable, IDamageable, IDamageDealer
{
    [SyncVar] private bool _isDead;
    [SyncVar, SerializeField] private float _health = 4;
    [SerializeField] private Sprite _icon;
    [SerializeField] private float _moveSpeed = 0.8f;
    [SerializeField] private float _attackRate = 1f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _cost = 5;
    [SerializeField] private Elements _element;

    private float _nextAttackTime = 0f;
    private IEnemyChecker _enemyChecker;
    private CharacterStateMachine _stateMachine;
    private Path _path;
    private List<Path> _paths;

    public Sprite Icon => _icon;
    public bool IsSelected { get; private set; }
    public float TempDamageValue { get; set; }
    public float Health { get { return _health; } private set { HPChanged?.Invoke(Health, value); RpcHPChanged(Health, value); _health = value; } }
    public bool IsCanTakeDamage => !_isDead;
    public Path Path => _path;
    public float MoveSpeed => _moveSpeed;
    public IEnemyChecker EnemyChecker => _enemyChecker;
    public GameObject Self => gameObject;
    public List<Path> Paths { get => _paths; set => _paths = value; }
    public Elements Element => _element;
    public float Cost { get => _cost; }
    public float Damage { get => _damage; }
    public float AttackRate { get => _attackRate; }

    public event Action<ISelectable> Selected;
    public event Action<ISelectable> Deselected;
    public event Action<IDamageable> BeforDamageTaked;
    public event Action<IDamageable, float> DamageTaked;
    public event Action<Damage> Died;
    public event Action<float, float> HPChanged;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned == false)
            return;

        _path = Game.Instance.OwnerBase.CurrentPath;

        _enemyChecker = GetComponentInChildren<IEnemyChecker>();
        _stateMachine = new(this);
    }

    private void Update()
    {
        if (isOwned == false || _isDead)
            return;

        _stateMachine.Update();
    }

    public void Deselect()
    {
        Deselected?.Invoke(this);
    }

    public void Select()
    {
        Selected?.Invoke(this);
    }

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

    public void SetPath(Path path)
    {
        _path = path;
    }

    public bool TryAttack(IDamageable target)
    {
        if (Time.time > _nextAttackTime)
        {
            _nextAttackTime = Time.time + _attackRate;
            DealDamage(target.Self);
            return true;
        }
        return false;
    }

    [Command]
    public void DealDamage(GameObject target)
    {
        Damage damage = new(_damage, this, target.GetComponent<IDamageable>());
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
