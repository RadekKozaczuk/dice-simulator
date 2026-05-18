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
                _roll.interactable = false;
            });
        }

        internal void SetValues(int result, int total)
        {
            _result.text = "Result: " + result;
            _total.text = "Total " + total;
        }

        /// <summary>
        /// Make the Roll button interactable.
        /// </summary>
        internal void EnableRoll() => _roll.interactable = true;
    }
}