#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using TMPro;
using UnityEngine;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class LeaderBoardElementView : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _playerName;

        [SerializeField]
        TextMeshProUGUI _score;

        internal void Initialize(string playerName, int playerScore)
        {
            _playerName.text = playerName;
            _score.text = playerScore.ToString();
        }
    }
}