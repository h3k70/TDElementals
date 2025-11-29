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
        foreach((RefRW<LocalTransform> localTransform, RefRO<MoveSpeed> moveSpeed, RefRW<PhysicsVelocity> physicsVelocity) 
            in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>, RefRW<PhysicsVelocity>>())
        {
            float3 targetPosition = MouseToWorldPosition.Instance.GetPosition();
            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            physicsVelocity.ValueRW.Linear = moveDirection * moveSpeed.ValueRO.moveSpeed;
            physicsVelocity.ValueRW.Angular = float3.zero;
            //localTransform.ValueRW.Position = localTransform.ValueRW.Position + new float3(moveSpeed.ValueRO.moveSpeed, 0, 0) * SystemAPI.Time.DeltaTime;
        }
    }
}
