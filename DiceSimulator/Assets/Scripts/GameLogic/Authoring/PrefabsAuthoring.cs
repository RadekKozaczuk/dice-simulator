#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using GameLogic.Components;
using Unity.Entities;
using UnityEngine;

namespace GameLogic.Authoring
{
    /// <summary>
    /// In DOTS prefabs are stored in a dedicated Entity instead of a config.
    /// All other values are normally accessible from configs.
    /// </summary>
    [DisallowMultipleComponent]
    class PrefabsAuthoring : MonoBehaviour
    {
        class Baker : Baker<PrefabsAuthoring>
        {
            public override void Bake(PrefabsAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PrefabsComponent());
            }
        }
    }
}