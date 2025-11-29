using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientRequestGameEntrySysyem : ISystem
{
    private EntityQuery _pendingNetworkIdQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
        _pendingNetworkIdQuery = state.GetEntityQuery(builder);
        state.RequireForUpdate(_pendingNetworkIdQuery);
        state.RequireForUpdate<GameEntryRequestData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var requestGameEntry = SystemAPI.GetSingleton<GameEntryRequestData>();
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var pendingNetworkIds = _pendingNetworkIdQuery.ToEntityArray(Allocator.Temp);

        foreach (var pendingNetworkId in pendingNetworkIds)
        {
            entityCommandBuffer.AddComponent<NetworkStreamInGame>(pendingNetworkId);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
