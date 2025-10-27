using UnityEngine;
using UnityEngine.AI;

public class PushBaseSatate : IState
{
    private Character _character;
    private IStateSwitcher _stateMachine;
    private Path _path;
    private NavMeshAgent _mover;
    private float _offset = 1f;

    public PushBaseSatate(Character character, IStateSwitcher stateMachine)
    {
        _character = character;
        _path = character.Path;
        _mover = character.GetComponent<NavMeshAgent>();
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _path = _character.Path;
        _mover.SetDestination(_path.Points[^1].position);
    }

    public void Exit()
    {
        _mover.SetDestination(_character.transform.position);
    }

    public void Update()
    {
        if (Vector3.Distance(_character.transform.position, _path.Points[^1].position) <= _offset)
        {
            OnReachedEndPoint();
        }
    }

    private void OnReachedEndPoint()
    {
        _stateMachine.SwitchState<AttackBaseBuildState>();
    }
}
