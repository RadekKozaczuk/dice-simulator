#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Unity.Entities;
using UnityEngine;

namespace GameLogic.Authoring
{
    class DestructionAreaAuthoring : MonoBehaviour
    {
        class Baker : Baker<DestructionAreaAuthoring>
        {
            public override void Bake(DestructionAreaAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<DestructionAreaTag>(entity);
            }
        }
    }
}