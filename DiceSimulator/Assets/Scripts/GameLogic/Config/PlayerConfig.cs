#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameLogic.Config
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/GameLogic/PlayerConfig")]
    class PlayerConfig : ScriptableObject
    {
        [Range(1, 20)]
        [SerializeField]
        internal int BallSpeed = 10;

        [InfoBox("The amount of balls spawned on every shot.", InfoMessageType.None)]
        [SerializeField]
        internal int BallPerShotCount = 1;

        [InfoBox("The amount of balls player has in each round.", InfoMessageType.None)]
        [SerializeField]
        internal int BallPerGameCount = 3;
    }
}