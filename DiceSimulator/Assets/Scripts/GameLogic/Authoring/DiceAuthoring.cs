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
                AddComponent<DiceTag>(entity);
                AddComponent<NewlySpawnedTag>(entity);
            }
        }
    }
}