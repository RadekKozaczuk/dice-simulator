using Core.Dtos;
using Unity.Collections;
using Unity.Entities;

namespace GameLogic.Components
{
    public struct DiceComponent : IComponentData
    {
        public FixedList512Bytes<DiceFace> Faces;

        // ReSharper disable once UnusedParameter.Local
        public DiceComponent(int _ = 0) => Faces = new FixedList512Bytes<DiceFace>();
    }
}