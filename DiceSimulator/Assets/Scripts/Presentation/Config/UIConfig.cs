using UnityEngine;
using UnityEngine.InputSystem;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Config/Presentation/UIConfig")]
    class UIConfig : ScriptableObject
    {
        [SerializeField]
        internal InputActionAsset InputActionAsset;
    }
}