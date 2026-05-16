#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using TMPro;
using UnityEngine;

namespace Presentation.Views
{
    class InfoView : MonoBehaviour
    {
        [SerializeField]
        RectTransform _rect;

        [SerializeField]
        TMP_Text _infoText;

        string _initialText;
        Vector2 _originalAnchorMin; // for 16:9
        Vector2 _originalAnchorMax; // for 16:9
        float _originalDistance; // for 16:9

        void Awake()
        {
            _originalAnchorMin = _rect.anchorMin;
            _originalAnchorMax = _rect.anchorMax;
            _originalDistance = _originalAnchorMax.x - _originalAnchorMin.x;
        }

        /// <summary>
        /// Also sets the value to 0.
        /// </summary>
        internal void Initialize(string initialText)
        {
            _initialText = initialText;
            SetValue(0);

            int width = PresentationSceneReferenceHolder.GameplayCamera.pixelWidth;
            int height = PresentationSceneReferenceHolder.GameplayCamera.pixelHeight;
            float ratio = (float)width / height;

            // distance in 16:9
            float ratioRatio = ratio / 1.777f; // 0.75
            float minX = _originalAnchorMin.x;
            float newDistance = _originalDistance * (1 / ratioRatio);

            // the default
            if (Mathf.Approximately(ratio, 1.777777777777777777f))
            {
                _rect.anchorMin = _originalAnchorMin;
                _rect.anchorMax = _originalAnchorMax;
                return;
            }

            if (ratioRatio < 1)
            {
                float offset = 1 - ratioRatio; // 0.25
                float offsetHalf = offset * 0.5f; // 0.125

                float newMinX;
                if (minX < 0.5f)
                    newMinX = minX - offsetHalf;
                else
                    newMinX = minX + offsetHalf;

                _rect.anchorMin = new Vector2(newMinX, _originalAnchorMin.y);
                _rect.anchorMax = new Vector2(newMinX + newDistance, _originalAnchorMax.y);
            }

            // the UI was designed with 16:9 in mind hence the 1.777
            // everything outside of that has to be readjusted horizontally
        }

        internal void SetValue(int value) => _infoText.text = _initialText + value;
    }
}