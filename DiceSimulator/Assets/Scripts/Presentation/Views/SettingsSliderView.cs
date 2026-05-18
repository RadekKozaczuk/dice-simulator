using UnityEngine;

namespace Presentation.Views
{
    class SettingsSliderView : MonoBehaviour
    {
        [SerializeField]
        RectTransform _sliderRect;

        [SerializeField]
        RectTransform _handleRect;

        // This setup have to be in Start (not Awake) method
        // because otherwise it is overwritten by the slider script
        void Start()
        {
            float sliderHeight = RectTransformUtility.PixelAdjustRect(_sliderRect, UISceneReferenceHolder.Canvas).height;
            _handleRect.sizeDelta = new Vector2(sliderHeight, 0);
        }
    }
}