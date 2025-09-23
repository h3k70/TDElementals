using System.Collections.Generic;
using UnityEngine;

public class DefenceState : IState
{
    private Character _character;
    private IStateSwitcher _stateMachine;
    private Path _path;
    private Mover _mover;
    private List<Character> _enemies;
    private float _offset = 1.5f;

    public DefenceState(Character character, IStateSwitcher stateMachine)
    {
        _character = character;
        _path = character.Path;
        _enemies = character.EnemyChecker.Enemies;
        _mover = new(character.transform, character.MoveSpeed);
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _path = _character.Path;
        _mover.SetPoint(_path.Points[0].position);
        _mover.ReachedEndPoint += OnReachedEndPoint;
    }

    public void Exit()
    {
        _mover.ReachedEndPoint -= OnReachedEndPoint;
    }

    public void Update()
    {
        foreach (Character enemy in _enemies)
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
        
    }
}
