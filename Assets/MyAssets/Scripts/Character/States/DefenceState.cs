using System.Collections.Generic;
using UnityEngine.AI;

public class DefenceState : IState
{
    private Character _character;
    private IStateSwitcher _stateMachine;
    private Path _path;
    private NavMeshAgent _mover;
    private List<Character> _enemies;
    private float _offset = 1.5f;

    public DefenceState(Character character, IStateSwitcher stateMachine)
    {
        _character = character;
        _path = character.Path;
        _enemies = character.EnemyChecker.Enemies;
        _mover = character.GetComponent<NavMeshAgent>();
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _path = _character.Path;
        _mover.SetDestination(_path.Points[0].position);
    }

    public void Exit()
    {
        _mover.SetDestination(_character.transform.position);
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
        _mover.SetDestination(_path.Points[0].position);
    }
}
