public class Minion : Character
{
    protected override void OnStart()
    {
        if (isServer)
            _stateMachine = new MinionStateMachine(this);
    }
}
