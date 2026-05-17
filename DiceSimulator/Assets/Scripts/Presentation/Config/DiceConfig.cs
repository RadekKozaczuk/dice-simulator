#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Presentation.Views;
using UnityEngine;

namespace Presentation.Config
{
    /// <summary>
    /// Bricks are static elements of the map.
    /// </summary>
    [CreateAssetMenu(fileName = "DiceConfig", menuName = "Config/Presentation/DiceConfig")]
    class DiceConfig : ScriptableObject
    {
        [SerializeField]
        internal DiceView Dice;
    }
}