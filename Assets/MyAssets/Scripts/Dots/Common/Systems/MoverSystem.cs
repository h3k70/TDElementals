using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct MoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRO<MoveDirection> moveDir, RefRO<MoveSpeed> moveSpeed, RefRW<PhysicsVelocity> physicsVelocity) 
            in SystemAPI.Query<RefRO<MoveDirection>, RefRO<MoveSpeed>, RefRW<PhysicsVelocity>>())
        {
            physicsVelocity.ValueRW.Linear = moveDir.ValueRO.Value * moveSpeed.ValueRO.Value;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
    }
}
