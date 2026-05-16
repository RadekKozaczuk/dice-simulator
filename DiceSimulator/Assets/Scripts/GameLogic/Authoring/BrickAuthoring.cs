#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using Core;
using GameLogic.Components;
using GameLogic.Services;
using Sirenix.OdinInspector;
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

        [SerializeField]
        bool _indestructible;

        // hide when bomb
        [HideIf("@this._indestructible == true")]
        [SerializeField]
        int _hp;

        [SerializeField]
        bool _invisible;

        class Baker : Baker<BrickAuthoring>
        {
            public override void Bake(BrickAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.NonUniformScale);
                BrickType type = authoring._type;
                int id = IdCounterService.NextId(type);
                var transform = authoring.GetComponent<Transform>();

                switch (type)
                {
                    case BrickType.Basic:
                    {
                        int hp = authoring._hp == 0 ? int.MinValue : authoring._hp;
                        float rotation = transform.rotation.eulerAngles.y;
                        float scale = transform.localScale.x;
                        AddComponent(entity, new BrickComponent(id, type, rotation, scale, hp));
                        
                        if (authoring._indestructible)
                            AddComponent(entity, new IndestructibleTag());

                        if (authoring._invisible)
                            AddComponent(entity, new InvisibilityTag());

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