using System;
using UnityEngine;
using UnityEngine.AI;

public class PassiveState : IState
{
    private Character _character;
    private IStateSwitcher _stateMachine;
    private IDamageDealer _target;

    public PassiveState(Character character, IStateSwitcher stateMachine)
    {
        _character = character;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _character.DamageTaked += OnDamageTaked;
    }

    public void Exit()
    {
        _character.DamageTaked -= OnDamageTaked;
        _target = null;
    }

    public void Update()
    {
        if (_target == null)
            return;

        if (_target is Character character && character.IsCanTakeDamage)
        {
            _character.TryAttack(character);
        }
    }

    private void OnDamageTaked(Damage damage)
    {
        _target = damage.DamageDealer;
    }
}
