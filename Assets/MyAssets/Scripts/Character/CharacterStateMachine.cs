using System.Collections.Generic;
using System.Linq;

public class CharacterStateMachine : IStateSwitcher
{
    private List<IState> _states;
    private IState _currentState;
    private Character _character;

    public CharacterStateMachine(Character character)
    {
        _character = character;

        _states = new List<IState>()
        {
            new StandartAttackState(character, this),
            new AttackBaseBuildState(character),
            new DefenceState(character, this),
            new PushBaseSatate(character, this),
            new AttackWeakElementState(character, this)
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