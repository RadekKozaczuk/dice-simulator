using Core;
using UnityEngine;

namespace Presentation.Popups.Views
{
    [DisallowMultipleComponent]
    abstract class AbstractPopup : MonoBehaviour
    {
        internal readonly PopupType Type;

        protected AbstractPopup(PopupType type) => Type = type;

        internal virtual void Initialize()
        {
            RectTransform rect = GetComponent<RectTransform>();
            SetPopupHeightSize(rect);
        }

        internal virtual void Close() { }

        static void SetPopupHeightSize(RectTransform rect)
        {
            Rect r = rect.rect;
            float scaleMultiplierY = r.size.y / r.size.x;
            float newHeightSize = r.width * scaleMultiplierY;

            Vector2 deltaSize = new Vector2(r.size.x, newHeightSize) - r.size;

            Vector2 pivot = rect.pivot;
            rect.offsetMin -= new Vector2(deltaSize.x * pivot.x, deltaSize.y * pivot.y);
            pivot = rect.pivot;
            rect.offsetMax += new Vector2(deltaSize.x * (1f - pivot.x), deltaSize.y * (1f - pivot.y));
        }
    }
}