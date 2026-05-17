#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using Unity.Entities;

namespace GameLogic.Components
{
    struct BrickComponent : IComponentData
    {
        internal readonly int Id;
        internal readonly BrickType Type;

        /// <summary>
        /// Amount of hits after which the brick is destroyed.
        /// <see cref="int.MinValue"/> means that brick is undestructable.
        /// </summary>
        internal readonly float Rotation;

        internal readonly float Scale;
        internal int Hp;

        internal BrickComponent(int id, BrickType type, float rotation, float scale, int hp = int.MinValue)
        {
            Id = id;
            Type = type;
            Hp = hp;
            Rotation = rotation;
            Scale = scale;
        }
    }
}