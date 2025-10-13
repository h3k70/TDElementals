using Mirror;

public class User : NetworkBehaviour
{
    public static User Instance;

    public override void OnStartClient()
    {
        if (isLocalPlayer && isOwned)
        {
            Instance = this;
        }
    }
}
