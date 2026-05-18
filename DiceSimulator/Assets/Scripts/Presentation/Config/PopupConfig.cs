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