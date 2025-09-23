using UnityEngine;

public interface IStateSwitcher
{
    public void SwitchState<T>() where T : IState;
}
