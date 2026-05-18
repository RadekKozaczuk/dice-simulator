using GameLogic.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views
{
    class PanelView : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _result;

        [SerializeField]
        TextMeshProUGUI _total;

        [SerializeField]
        Button _roll;

        void Awake()
        {
            _roll.onClick.AddListener(() =>
            {
                GameLogicViewModel.StartRoll();
                _roll.enabled = false;
            });
        }

        internal void OnDiceStopped()
        {
            _roll.enabled = true;
        }
    }
}