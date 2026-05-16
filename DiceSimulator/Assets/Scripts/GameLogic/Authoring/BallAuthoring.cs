#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using GameLogic.Components;
using Unity.Entities;
using UnityEngine;

namespace GameLogic.Authoring
{
    class BallAuthoring : MonoBehaviour
    {
        class Baker : Baker<BallAuthoring>
        {
            public override void Bake(BallAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<BallComponent>(entity);
            }
        }
    }
}