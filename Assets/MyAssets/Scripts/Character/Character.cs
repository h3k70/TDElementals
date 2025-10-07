using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class Character : NetworkBehaviour, ISelectable, IDamageable, IDamageDealer, IHaveLVL
{
    [SyncVar] private int _lvl = 0;
    [SyncVar] private float _currentExp = 0;
    [SyncVar] private float _expForNextLvl = 0;
    [SyncVar] private bool _isDead;
    [SyncVar, SerializeField] private float _health = 4;
    [SerializeField] private Sprite _icon;
    [SyncVar, SerializeField] private float _moveSpeed = 0.8f;
    [SyncVar, SerializeField] private float _attackRate = 1f;
    [SyncVar, SerializeField] private float _damage = 1f;
    [SyncVar, SerializeField] private float _cost = 10;
    [SerializeField] private Elements _element;
    [SerializeField] private Animator _animator;
    [SerializeField] private NetworkAnimator _netAnimator;
    [SerializeField] private Collider _selectCollider;

    private float _addExpMultipleForNextLvl = 5;
    private float _addCostForNextLvl = 5;
    private float _costBase = 10;
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
    public Character SelfCard { get; set; }
    public List<Path> Paths { get => _paths; set => _paths = value; }
    public Elements Element => _element;
    public float Cost { get => _cost; }
    public float Damage { get => _damage; }
    public float AttackRate { get => _attackRate; }
    public int CurrentLVL => _lvl;
    public int MaxLVL => 10;
    public float CurrentExp => _currentExp;
    public float ExpForNextLVL => _expForNextLvl;

    public event Action<ISelectable> Selected;
    public event Action<ISelectable> Deselected;
    public event Action<IDamageable> BeforDamageTaked;
    public event Action<IDamageable, float> DamageTaked;
    public event Action<Damage> Died;
    public event Action<float, float> HPChanged;
    public event Action<int, int> LVLChanged;
    public event Action<float, float> ExpChanged;
    public event Action<float> CostChanged;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned == false || Game.Instance.OwnerBase == null)
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

            if (Health <= 0)
            {
                _isDead = true;
                Health = 0;
                Died?.Invoke(damage);
                RpcDied(damage.Value, damage.Damageable.Self, damage.DamageDealer.Self);
            }
            DamageTaked?.Invoke(this, damage.Value);
            RpcDamageTaked(damage.Value);
        }
    }

    public void SetPath(Path path)
    {
        _path = path;
    }

    public void SetMode(UnitCommands mode)
    {
        switch (mode)
        {
            case UnitCommands.MoveAndAttak:
                _stateMachine.SwitchState<StandartAttackState>();
                break;

            case UnitCommands.PushBase:
                _stateMachine.SwitchState<PushBaseSatate>();
                break;

            case UnitCommands.Defense:
                _stateMachine.SwitchState<DefenceState>();
                break;

            default:
                break;
        }
    }

    public bool TryAttack(IDamageable target)
    {
        if (Time.time > _nextAttackTime)
        {
            _nextAttackTime = Time.time + _attackRate;

            transform.LookAt(target.Self.transform.position);
            _animator.SetTrigger("Slice Attack");
            _netAnimator.SetTrigger("Slice Attack");
            DealDamage(target.Self);
            return true;
        }
        return false;
    }

    public void SetLVL(int lvl)
    {
        if (lvl <= MaxLVL)
        {
            var oldLvl = _lvl;
            _lvl = lvl;

            _cost = _costBase + (lvl - 1) * _addCostForNextLvl;
            _expForNextLvl = lvl * _addExpMultipleForNextLvl;

            ExpChanged?.Invoke(_currentExp, _expForNextLvl);
            LVLChanged?.Invoke(oldLvl, _lvl);
            CostChanged?.Invoke(_cost);

            RpcExpChanged(_currentExp, _expForNextLvl);
            RpcLVLChanged(oldLvl, _lvl);
            RpcCostChanged(_cost);
        }
    }

    public void AddExp(float value)
    {
        var total = _currentExp + value;      

        if (total >= _expForNextLvl)
        {
            total -= _expForNextLvl;
            SetLVL(_lvl + 1);
            
        }
        _currentExp = total;    
        ExpChanged?.Invoke(_currentExp, _expForNextLvl);

        RpcExpChanged(_currentExp, _expForNextLvl);
    }

    public void Buff(Buffs buff, float value)
    {
        switch (buff)
        {
            case Buffs.Damage:
                _damage *= value;
                break;
            case Buffs.AttackSpeed:
                _attackRate /= value;
                break;
            case Buffs.MoveSpeed:
                _moveSpeed *= value;
                break;
            default:
                break;
        }
    }

    public void Debuff(Buffs buff, float value)
    {
        switch (buff)
        {
            case Buffs.Damage:
                _damage /= value;
                break;
            case Buffs.AttackSpeed:
                _attackRate *= value;
                break;
            case Buffs.MoveSpeed:
                _moveSpeed /= value;
                break;
            default:
                break;
        }
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

    [ClientRpc]
    private void RpcDied(float value, GameObject damageable, GameObject damageDealer)
    {
        Damage damage = new()
        {
            Value = value,
            Damageable = damageable.GetComponent<IDamageable>(),
            DamageDealer = damageDealer.GetComponent<IDamageDealer>(),
        };

        Died?.Invoke(damage);
        _animator.SetTrigger("Die");
        _netAnimator.SetTrigger("Die");
        _selectCollider.enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
    }

    [ClientRpc]
    private void RpcExpChanged(float currentExp, float expForNextLvl)
    {
        ExpChanged?.Invoke(currentExp, expForNextLvl);
    }
    
    [ClientRpc]
    private void RpcLVLChanged(int oldLvl, int lvl)
    {
        LVLChanged?.Invoke(oldLvl, lvl);
    }

    [ClientRpc]
    private void RpcCostChanged(float cost)
    {
        CostChanged?.Invoke(cost);
    }
}
