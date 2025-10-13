using Mirror;

public class MultiplayerManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Game.Instance.AddPlayer(conn);
    }
}
