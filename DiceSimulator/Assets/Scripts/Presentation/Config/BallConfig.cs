#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Presentation.Views;
using UnityEngine;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "CoinConfig", menuName = "Config/Presentation/CoinConfig")]
    class BallConfig : ScriptableObject
    {
        [SerializeField]
        internal BallView Prefab;

        [SerializeField]
        internal float MinRotationSpeed = 50f;

        [SerializeField]
        internal float MaxRotationSpeed = 150f;

        [SerializeField]
        internal float YOffset = 0.5f;
    }
}