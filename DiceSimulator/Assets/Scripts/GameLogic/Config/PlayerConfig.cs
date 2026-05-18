#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameLogic.Config
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/GameLogic/PlayerConfig")]
    class PlayerConfig : ScriptableObject
    {
        [InfoBox("Force at which the dice is thrown when using RollButton.", InfoMessageType.None)]
        [Range(1, 50)]
        [SerializeField]
        internal int AutoRollStrength = 15;

        [InfoBox("The higher the value the faster dice's physics gets disabled."
            + " Used to prevent the dice from wiggling when on the ground."
            + " Adjust accordingly. Too high values and the dice may stop preemptively, too low and it may never stop.", InfoMessageType.None)]
        [Range(0, 1f)]
        [SerializeField]
        internal float DicePhysicsFreezeThreshold = 0.1f;

        [Range(1, 20)]
        [InfoBox("Height at which the dice is initially lift up when rolled by player.", InfoMessageType.None)]
        [SerializeField]
        internal float DiceHeight = 10;

        [Range(0, 5)]
        [InfoBox("How long it will take the dice to reach the maximum height after being lifted up manually by player.", InfoMessageType.None)]
        [SerializeField]
        internal float AscendingTime = 2;
    }
}