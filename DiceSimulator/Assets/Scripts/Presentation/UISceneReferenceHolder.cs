#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Presentation.Views;
using UnityEngine;

namespace Presentation
{
    /// <summary>
    /// Keep in mind references stored here are only accessible AFTER UI scene is fully loaded up.
    /// Use, for example, <see cref="Controllers.PresentationMainController._uiSceneLoaded" /> to control the execution.
    /// </summary>
    class UISceneReferenceHolder : MonoBehaviour
    {
        internal static Canvas Canvas;
        internal static Transform PopupContainer;
        internal static Transform HpLabelContainer;
        internal static InfoView BallsLeft;
        internal static InfoView Score;

        [SerializeField]
        Canvas _canvas;

        [SerializeField]
        Transform _popupContainer;

        [SerializeField]
        Transform _hpLabelContainer;

        [SerializeField]
        InfoView _ballsLeft;

        [SerializeField]
        InfoView _score;

        void Awake()
        {
            Canvas = _canvas;
            PopupContainer = _popupContainer;
            HpLabelContainer = _hpLabelContainer;
            BallsLeft = _ballsLeft;
            Score = _score;
        }
    }
}