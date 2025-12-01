using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MoveDirectionAuthoring : MonoBehaviour
{
    public float3 direction;

    public class Baker : Baker<MoveDirectionAuthoring>
    {
        public override void Bake(MoveDirectionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveDirection { });
        }
    }
}
