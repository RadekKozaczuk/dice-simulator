#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Unity.Entities;
using UnityEngine;

namespace GameLogic.Authoring
{
    class SpawnPointAuthoring : MonoBehaviour
    {
        class Baker : Baker<SpawnPointAuthoring>
        {
            public override void Bake(SpawnPointAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<SpawnPointTag>(entity);
            }
        }
    }
}