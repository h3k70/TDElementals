using System.Collections.Generic;
using Unity.Services.Multiplay.Authoring.Core.MultiplayApi;
using UnityEngine;

public enum Servers
{
    None = 0,
    MainServer = 1,
    Localhost = 2,
}

public class HTTPLibrary : MonoBehaviour
{
    public static readonly Dictionary<Servers, string> ServerIP = new Dictionary<Servers, string>()
    {
        {Servers.Localhost, "127.0.0.1"},
        {Servers.MainServer, "89.169.1.90"},
    };

    public static string GetIP(Servers ip)
    {
        return ServerIP.GetValueOrDefault(ip);
    }
}
