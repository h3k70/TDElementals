using System.Collections.Generic;
using Unity.Services.Multiplay.Authoring.Core.MultiplayApi;
using UnityEngine;

public enum Servers
{
    None = 0,
    MainServer = 1,
    Localhost = 2,
}

public enum ConnectMode
{
    None = 0,
    Server = 1,
    Client = 2,
    ServerAndClient = 3,
}

public class HTTPLibrary : MonoBehaviour
{
    private const ushort PORT = 7070;

    public static readonly Dictionary<Servers, string> ServerIP = new Dictionary<Servers, string>()
    {
        {Servers.Localhost, "127.0.0.1"},
        {Servers.MainServer, "89.169.1.90"},
    };

    public static ushort Port { get { return PORT; } }

    public static string GetIP(Servers ip)
    {
        return ServerIP.GetValueOrDefault(ip);
    }
}
