#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using GameLogic.Components;
using Unity.Entities;
using UnityEngine;

namespace GameLogic.Authoring
{
    class DiceAuthoring : MonoBehaviour
    {
        class Baker : Baker<DiceAuthoring>
        {
            public override void Bake(DiceAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<DiceComponent>(entity);
                AddComponent<NewlySpawnedTag>(entity);
            }
        }
    }
}