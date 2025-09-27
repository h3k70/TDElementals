using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PushBaseSatate : IState
{
    private Character _character;
    private IStateSwitcher _stateMachine;
    private Path _path;
    private PathMover _mover;
    private float _offset = 1.5f;

    public PushBaseSatate(Character character, IStateSwitcher stateMachine)
    {
        _character = character;
        _path = character.Path;
        _mover = new PathMover(character.transform, character.MoveSpeed, character.Path, _offset);
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _path = _character.Path;
        _mover.SetPath(_path);
        _mover.ReachedEndPoint += OnReachedEndPoint;
    }

    public void Exit()
    {
        _mover.ReachedEndPoint -= OnReachedEndPoint;
    }

    public void Update()
    {
        _mover.Update();
    }

    private void OnReachedEndPoint()
    {
        _stateMachine.SwitchState<AttackBaseBuildState>();
    }
}
