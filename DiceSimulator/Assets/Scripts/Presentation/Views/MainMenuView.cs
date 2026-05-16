#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using Core.Services;
using Presentation.Popups;
using Presentation.ViewModels;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class MainMenuView : MonoBehaviour
    {
        [SerializeField]
        Button _newGame;

        [SerializeField]
        Button _settings;

        [SerializeField]
        Button _quit;

        void Awake()
        {
            _newGame.onClick.AddListener(NewGame);
            _settings.onClick.AddListener(Settings);
            _quit.onClick.AddListener(Quit);
        }

        static void NewGame()
        {
            PresentationViewModel.PlaySound(Sound.ClickHit);
            GameStateService.ChangeState(GameState.Gameplay);
        }

        static void Settings()
        {
            PresentationViewModel.PlaySound(Sound.ClickHit);
            PopupService.ShowPopup(PopupType.Settings);
        }

        static void Quit()
        {
            PresentationViewModel.PlaySound(Sound.ClickHit);

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}