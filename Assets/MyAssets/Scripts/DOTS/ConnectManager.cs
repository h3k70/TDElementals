using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectManager
{
    private int _gameSceneIndex;

    public ConnectManager(int gameSceneIndex)
    {
        _gameSceneIndex = gameSceneIndex;
    }

    public void StartServerWorld(ushort port)
    {
        DestroyLocalSimulationWorld();
        SceneManager.LoadScene(_gameSceneIndex);

        var serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        var serverEndPoint = NetworkEndpoint.AnyIpv4.WithPort(port);
        {
            using var networkDriverQuery = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndPoint);
        }
    }

    public void StartClientWorld(string ipAddress, ushort port)
    {
        DestroyLocalSimulationWorld();
        SceneManager.LoadScene(_gameSceneIndex);

        var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
        var connectionEndpoint = NetworkEndpoint.Parse(ipAddress, port);
        {
            using var networkDriverQuery = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, connectionEndpoint);
        }

        World.DefaultGameObjectInjectionWorld = clientWorld;
    }

    private void DestroyLocalSimulationWorld()
    {
        foreach (var world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }
    }
}
