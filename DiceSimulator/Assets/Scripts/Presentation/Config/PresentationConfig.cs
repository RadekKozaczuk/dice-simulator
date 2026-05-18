using Presentation.Views;
using UnityEngine;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "PresentationConfig", menuName = "Config/Presentation/PresentationConfig")]
    class PresentationConfig : ScriptableObject
    {
        [SerializeField]
        internal DiceView DicePrefab;

        [SerializeField]
        internal GameObject GroundPrefab;
    }
}