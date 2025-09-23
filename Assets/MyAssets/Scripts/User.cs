using System.Collections.Generic;
using Mirror;
using UnityEngine;

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
