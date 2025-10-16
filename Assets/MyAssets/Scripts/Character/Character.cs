using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class Character : NetworkBehaviour, ISelectable, IDamageable, IDamageDealer, IHaveLVL
{
    [SyncVar] private int _lvl = 0;
    [SyncVar] private float _currentExp = 0;
    [SyncVar] private float _totalExp = 0;
    [SyncVar] private float _expForNextLvl = 0;
    [SyncVar] private bool _isDead;
    [SyncVar, SerializeField] private float _health = 4;
    [SyncVar, SerializeField] private float _maxHealth = 4;
    [SyncVar, SerializeField] private float _regenHpValue = 0f;
    [SerializeField] private Sprite _icon;
    [SyncVar, SerializeField] private float _moveSpeed = 0.8f;
    [SyncVar, SerializeField] private float _attackRate = 1f;
    [SyncVar, SerializeField] private float _damage = 1f;
    [SyncVar, SerializeField] private float _cost = 10;
    [SerializeField] private Elements _element;
    [SerializeField] private Animator _animator;
    [SerializeField] private NetworkAnimator _netAnimator;
    [SerializeField] private Collider _selectCollider;

    private float _regenHpRate = 1f;
    private float _addExpMultipleForNextLvl = 5;
    private float _addCostForNextLvl = 5;
    private float _costBase = 10;
    private float _nextAttackTime = 0f;
    private IEnemyChecker _enemyChecker;
    private CharacterStateMachine _stateMachine;
    private Path _path;
    private List<Path> _paths;
    private UnitCommands _command;
    private float _percentUpAttributes = 0.1f;

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
    public float TotalExp { get => _totalExp; }

    public event Action<ISelectable> Selected;
    public event Action<ISelectable> Deselected;
    public event Action<IDamageable> BeforDamageTaked;
    public event Action<IDamageable, float> DamageTaked;
    public event Action<Damage> Died;
    public event Action<Character> CharacterDied;
    public event Action<float, float> HPChanged;
    public event Action<float, float> MaxHPChanged;
    public event Action<int, int> LVLChanged;
    public event Action<float, float> ExpChanged;
    public event Action<float> CostChanged;
    public event Action<Buffs> BuffAdded;
    public event Action<Buffs> BuffRemoved;

    private void Start()
    {
        if (isServer)
        {
            StartCoroutine(RegenHPJob());
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        HPChanged?.Invoke(_health, _health);
        MaxHPChanged?.Invoke(_maxHealth, _maxHealth);

        if (isOwned == false || Game.Instance.OwnerBase == null)
        {
            return;
        }

        _path = Game.Instance.OwnerBase.CurrentPath;

        _enemyChecker = GetComponentInChildren<IEnemyChecker>();
        _stateMachine = new(this);
        _command = UnitCommands.MoveAndAttak;
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
        if (_path == path)
            return;

        _path = path;
        SetMode(_command);
    }

    public void SetMode(UnitCommands mode)
    {
        _command = mode;

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

            case UnitCommands.MoveAndAttackWeak:
                _stateMachine.SwitchState<AttackWeakElementState>();
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

            _health += _lvl * _percentUpAttributes * _health;
            _maxHealth += _lvl * _percentUpAttributes * _maxHealth;
            RpcHPChanged(_health, _health);
            RpcMaxHPChanged(_maxHealth, _maxHealth);

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
        _totalExp += value;

        if (total >= _expForNextLvl)
        {
            total -= _expForNextLvl;
            SetLVL(_lvl + 1);
            
        }
        _currentExp = total;    
        ExpChanged?.Invoke(_currentExp, _expForNextLvl);

        RpcExpChanged(_currentExp, _expForNextLvl);
    }

    public void TakeExp(float value)
    {
        _totalExp -= value;
        var total = _currentExp - value;

        if (total < 0)
        {
            if (_lvl - 1 <= 0)
                return;

            total = _expForNextLvl - total;
            SetLVL(_lvl - 1);

            LVLChanged?.Invoke(_lvl - 1, _lvl);
            RpcLVLChanged(_lvl - 1, _lvl);
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
        BuffAdded?.Invoke(buff);
        RpcBuffAdded(buff);
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
        BuffRemoved?.Invoke(buff);
        RpcBuffRemoved(buff);
    }

    private IEnumerator RegenHPJob()
    {
        var time = new WaitForSeconds(_regenHpRate);

        while (_isDead == false)
        {
            yield return time;

            if (_health + _regenHpValue > _maxHealth)
            {
                RpcHPChanged(_health, _maxHealth);
                _health = _maxHealth;
            }
            else
            {
                RpcHPChanged(_health, _health + _regenHpValue);
                _health += _regenHpValue;
            }
            
        }
    }

    private IEnumerator DieDissolve()
    {
        yield return new WaitForSeconds(5);
        transform.DOLocalMoveY(-1, 15);
        Destroy(gameObject, 15);
    }

    [Command]
    public void CmdPayExpCost(float value)
    {
        TakeExp(value);
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
    private void RpcMaxHPChanged(float oldValue, float newValue)
    {
        MaxHPChanged?.Invoke(oldValue, newValue);
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
        CharacterDied?.Invoke(this);
        _animator.SetTrigger("Die");
        _netAnimator.SetTrigger("Die");
        _selectCollider.enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        StartCoroutine(DieDissolve());
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

    [ClientRpc]
    private void RpcBuffAdded(Buffs buff)
    {
        BuffAdded?.Invoke(buff);
    }

    [ClientRpc]
    private void RpcBuffRemoved(Buffs buff)
    {
        BuffRemoved?.Invoke(buff);
    }
}
