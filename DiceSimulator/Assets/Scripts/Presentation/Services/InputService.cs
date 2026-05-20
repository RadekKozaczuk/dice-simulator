using Core;
using GameLogic.ViewModels;
using Presentation.Config;
using Presentation.Popups;
using Presentation.ViewModels;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Presentation.Services
{
    static class InputService
    {
        const string Quit = "Quit";
        const string Move = "Move";
        const string Click = "Click";

        static readonly UIConfig _uiConfig;

        internal static void Initialize()
        {
            // MainMenu bindings
            InputActionMap mainMenu = _uiConfig.InputActionAsset.FindActionMap(Constants.MainMenuActionMap);
            mainMenu.FindAction(Quit).performed += static _ =>
            {
                // if there is a popup - close it
                // otherwise quit the game
                if (PopupService.CurrentPopup)
                    PopupService.CloseCurrentPopup();
                else
                {
                    GameLogicViewModel.QuitGame();

#if UNITY_EDITOR
                    UnityEditor.EditorApplication.ExitPlaymode();
#else
                    Application.Quit();
#endif
                }
            };

            // Gameplay bindings
            InputActionMap gameplay = _uiConfig.InputActionAsset.FindActionMap(Constants.GameplayActionMap);
            gameplay.FindAction(Quit).performed += static _ => PopupService.ShowPopup(PopupType.QuitGame);

            InputAction moveAction = gameplay.FindAction(Move);
            moveAction.performed += _ =>
            {
                Vector2 mousePosition = moveAction.ReadValue<Vector2>();
                PresentationViewModel.SetMousePosition(mousePosition);
            };

            InputAction clickAction = gameplay.FindAction(Click);
            clickAction.performed += _ =>
            {
                Vector2 mousePosition = moveAction.ReadValue<Vector2>();
                PresentationViewModel.TryGrabDice(mousePosition);
            };

            clickAction.canceled += static _ => PresentationViewModel.ReleaseDice();

            // Popups bindings
            InputActionMap popup = _uiConfig.InputActionAsset.FindActionMap(Constants.PopupActionMap);
            popup.FindAction(Quit).performed += static _ =>
            {
                if (PopupService.CurrentPopup)
                    PopupService.CloseCurrentPopup();
                else
                    PopupService.ShowPopup(PopupType.QuitGame);
            };
        }
    }
}