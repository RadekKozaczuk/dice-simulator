#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using Core.CustomInspector;
using Presentation.Popups.Views;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "PopupConfig", menuName = "Config/UI/PopupConfig")]
    class PopupConfig : ScriptableObject
    {
        [InfoBox("Order should match Core.Enums.PopupType enum.", InfoMessageType.None)]
        [SerializeField]
        [LabeledArray(typeof(PopupType))]
        internal AbstractPopup[] PopupPrefabs;

        [SerializeField]
        internal Image BlockingPanelPrefab;
    }
}