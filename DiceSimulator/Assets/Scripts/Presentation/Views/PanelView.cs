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
                GameLogicViewModel.AutoRoll();
                RollInProgress();
            });
        }

        /// <summary>
        /// Disables <see cref="_roll"/> button and changes the description of <see cref="_result"/>
        /// </summary>
        internal void RollInProgress()
        {
            _roll.interactable = false;
            _result.text = "Result: ?";
        }

        /// <summary>
        /// Make the Roll button interactable and prints the results.
        /// </summary>
        internal void RollEnded(int result, int total)
        {
            _roll.interactable = true;
            _result.text = "Result: " + result;
            _total.text = "Total " + total;
        }
    }
}