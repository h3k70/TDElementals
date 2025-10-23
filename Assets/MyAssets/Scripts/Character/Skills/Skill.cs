using System;
using System.Collections;
using Mirror;
using UnityEngine;

public abstract class Skill : NetworkBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    [SerializeField] private float _distence;
    [SerializeField] private float _cooldown;

    private bool _isReady = true;
    private Character _character;
    private ITargetable _target;
    private Coroutine _wrapperCastCoroutine;
    private Coroutine _castCoroutine;

    public string Name { get => _name; }
    public string Description { get => _description; }
    public Sprite Icon { get => _icon; }
    public Character Character { get => _character; }
    public bool IsReady { get => _isReady; }
    public bool IsTargetInRadius 
    { 
        get
        {
            return _target != null && Vector3.Distance(_character.transform.position, _target.Transform.position) <= _distence;
        }
    }
    public float Distence { get => _distence; }

    public event Action<Skill> CastStarted;
    public event Action<Skill> CastEnded;
    public event Action<Skill> CastCancled;
    public event Action<float, Skill> CooldownStarted;
    public event Action<Skill> CooldownEnded;

    protected abstract IEnumerator CastJob(ITargetable target);
    protected abstract void ClearData();

    public void Init(Character character)
    {
        _character = character;
    }

    public void SetTarget(ITargetable target)
    {
        _target = target;
    }

    public virtual bool TryCast(ITargetable target)
    {
        _target = target;

        if (_wrapperCastCoroutine == null && IsReady && IsTargetInRadius)
        {
            _isReady = false;
            StartCoroutine(CooldownJob());
            _wrapperCastCoroutine = StartCoroutine(WrapperCastJob(target));
            return true;
        }
        return false;
    }
    
    public bool TryCancel()
    {
        StopCorounineJob(_castCoroutine);
        StopCorounineJob(_wrapperCastCoroutine);

        _target = null;
        ClearData();

        CastCancled?.Invoke(this);
        return true;
    }

    private void StopCorounineJob(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private IEnumerator WrapperCastJob(ITargetable target)
    {
        CastStarted?.Invoke(this);

        yield return _castCoroutine = StartCoroutine(CastJob(target));
        _wrapperCastCoroutine = null;
        _castCoroutine = null;

        CastEnded?.Invoke(this);
    }

    private IEnumerator CooldownJob()
    {
        WaitForSeconds time = new(_cooldown);

        CooldownStarted?.Invoke(_cooldown, this);

        yield return time;
        _isReady = true;

        CooldownEnded?.Invoke(this);
    }
}
