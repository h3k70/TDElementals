public class Hero : Character
{
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned == false || Game.Instance.OwnerBase == null)
        {
            return;
        }
        Path = Game.Instance.OwnerBase.CurrentPath;
        EnemyChecker = GetComponentInChildren<IEnemyChecker>();
        _stateMachine = new CharacterStateMachine(this);
        CurrentCommand = UnitCommands.MoveAndAttak;
    }
}
