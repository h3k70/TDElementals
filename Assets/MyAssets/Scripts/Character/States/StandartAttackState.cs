using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class StandartAttackState : IState
{
    private Character _character;
    private IStateSwitcher _stateMachine;
    private Path _path;
    private PathMover _mover;
    private List<Character> _enemies;
    private float _offset = 1.5f;

    public StandartAttackState(Character character, IStateSwitcher stateMachine)
    {
        _character = character;
        _path = character.Path;
        _enemies = character.EnemyChecker.Enemies;
        _mover = new(character.transform, character.MoveSpeed, character.Path, _offset);
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _mover.SetPath(_path);
        _mover.ReachedEndPoint += OnReachedEndPoint;
    }

    public void Exit()
    {
        _mover = null;
    }

    public void Update()
    {
        foreach(Character enemy in _enemies)
        {
            if (enemy.IsCanTakeDamage)
            {
                _character.TryAttack(enemy);
                return;
            }
        }
        _mover.Update();
    }

    private void OnReachedEndPoint()
    {
        _stateMachine.SwitchState<AttackBaseBuildState>();
    }
}