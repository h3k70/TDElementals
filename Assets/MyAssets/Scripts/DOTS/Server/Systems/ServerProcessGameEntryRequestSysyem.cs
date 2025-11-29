using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct ServerProcessGameEntryRequestSysyem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<ReceiveRpcCommandRequest>();
        state.RequireForUpdate(state.GetEntityQuery(builder));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach(var (requestSource, requestEntity) in
            SystemAPI.Query<ReceiveRpcCommandRequest>().WithEntityAccess())
        {
            entityCommandBuffer.DestroyEntity(requestEntity);
            entityCommandBuffer.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

            var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;

            UnityEngine.Debug.Log(clientId);
        }
        entityCommandBuffer.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
