#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Presentation.Views;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Config/Presentation/UIConfig")]
    class UIConfig : ScriptableObject
    {
        [SerializeField]
        internal InputActionAsset InputActionAsset;

        [SerializeField]
        internal HpView HpLabel;
    }
}