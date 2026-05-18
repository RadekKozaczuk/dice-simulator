#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Presentation.Views;
using UnityEngine;

namespace Presentation
{
    /// <summary>
    /// Keep in mind references stored here are only accessible AFTER UI scene is fully loaded up.
    /// </summary>
    class UISceneReferenceHolder : MonoBehaviour
    {
        internal static Canvas Canvas;
        internal static Transform PopupContainer;
        internal static PanelView Panel;

        [SerializeField]
        Canvas _canvas;

        [SerializeField]
        Transform _popupContainer;

        [SerializeField]
        PanelView _panel;

        void Awake()
        {
            Canvas = _canvas;
            PopupContainer = _popupContainer;
            Panel = _panel;
        }
    }
}