#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using TMPro;
using UnityEngine;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class HpView : MonoBehaviour
    {
        internal int Hp { set => _hp.text = value.ToString(); }

        [SerializeField]
        TextMeshProUGUI _hp;
    }
}