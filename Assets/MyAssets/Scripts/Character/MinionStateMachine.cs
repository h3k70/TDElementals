using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinionStateMachine : IStateSwitcher
{
    private List<IState> _states;
    private IState _currentState;
    private Character _character;

    public MinionStateMachine(Character character)
    {
        _character = character;

        _states = new List<IState>()
        {
            new PassiveState(character, this)
        };

        _currentState = _states[0];
        _currentState.Enter();
    }

    public void SwitchState<T>() where T : IState
    {
        IState state = _states.FirstOrDefault(state => state is T);

        _currentState.Exit();
        _currentState = state;
        _currentState.Enter();
    }

    public void Update()
    {
        _currentState.Update();
    }
}
