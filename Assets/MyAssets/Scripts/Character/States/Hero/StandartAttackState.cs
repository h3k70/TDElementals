using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;

public class StandartAttackState : IState
{
    private Character _character;
    private IStateSwitcher _stateMachine;
    private Path _path;
    private Mover _mover;
    private List<ITargetable> _enemies;
    private bool _isMoving = false;
    private bool _isReachEndPoint = false;
    private Transform _currentTransform;
    private Vector3 _currentPoint;
    private float _offset = 1f;

    public StandartAttackState(Character character, IStateSwitcher stateMachine)
    {
        _character = character;
        _path = character.Path;
        _enemies = character.EnemyChecker.Enemies;
        _mover = character.Mover;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _isReachEndPoint = false;
        _isMoving = false;
        _path = _character.Path;
    }

    public void Exit()
    {

    }

    public void Update()
    {
        foreach(var enemy in _enemies)
        {
            if (enemy is Character character && character.IsCanTakeDamage)
            {
                _character.TryAttack(character);
                _isMoving = false;
                return;
            }
        }
        CalculatePath();
    }

    private void CalculatePath()
    {
        if (_isReachEndPoint)
            return;

        if (_isMoving == false)
        {
            _currentTransform = _path.GetNextPoint(_path.GetCloserPoint(_character.transform.position));

            if (_currentTransform != null)
            {
                _currentPoint = _currentTransform.position;
                _character.Mover.MoveTo(_currentPoint, _offset);
                _isMoving = true;
            }
            else
            {
                OnReachedEndPoint();
                _isReachEndPoint = true;
            }
        }
        if (Vector3.Distance(_character.transform.position, _currentPoint) <= _offset)
        {
            _isMoving = false;
        }
    }

    private void OnReachedEndPoint()
    {
        _stateMachine.SwitchState<AttackBaseBuildState>();
    }
}