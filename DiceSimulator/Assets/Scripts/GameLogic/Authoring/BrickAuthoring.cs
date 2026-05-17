#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using Core;
using GameLogic.Components;
using GameLogic.Services;
using Unity.Entities;
using UnityEngine;

namespace GameLogic.Authoring
{
    /// <summary>
    /// Handles both spawn points and portals.
    /// </summary>
    class BrickAuthoring : MonoBehaviour
    {
        [SerializeField]
        BrickType _type;

        class Baker : Baker<BrickAuthoring>
        {
            public override void Bake(BrickAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                BrickType type = authoring._type;
                int id = IdCounterService.NextId(type);
                Transform transform = authoring.GetComponent<Transform>();

                switch (type)
                {
                    case BrickType.Basic:
                    {
                        float rotation = transform.rotation.eulerAngles.y;
                        float scale = transform.localScale.x;
                        AddComponent(entity, new BrickComponent(id, type, rotation, scale, 0));

                        break;
                    }
                    case BrickType.Bomb:
                        AddComponent(entity, new BrickComponent());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                AddComponent(entity, new NewlySpawnedTag());
            }
        }
    }
}