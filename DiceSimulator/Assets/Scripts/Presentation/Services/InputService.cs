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
        const string MousePosition = "MousePosition";
        const string Roll = "Click";

        static readonly UIConfig _uiConfig;

        static InputAction _clickAction;
        static InputAction _mousePositionAction;

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

            _mousePositionAction = gameplay.FindAction(MousePosition);
            _mousePositionAction.performed += static _ =>
            {
                Vector2 mousePosition = _mousePositionAction.ReadValue<Vector2>();
                PresentationViewModel.SetMousePosition(mousePosition);
            };

            _clickAction = gameplay.FindAction(Roll);
            _clickAction.performed += static _ =>
            {
                Vector2 mousePosition = _mousePositionAction.ReadValue<Vector2>();
                PresentationViewModel.TryGrabDice(mousePosition);
            };

            _clickAction.canceled += static _ => PresentationViewModel.ReleaseDice();

            // Popups bindings
            InputActionMap popup = _uiConfig.InputActionAsset.FindActionMap(Constants.PopupActionMap);
            popup.FindAction(Quit).performed += static _ =>
            {
                if (PopupService.CurrentPopup)
                    PopupService.CloseCurrentPopup();
                else
                    PopupService.ShowPopup(PopupType.QuitGame);
            };

            _uiConfig.InputActionAsset.FindActionMap(Constants.GameplayActionMap).Disable();
        }
    }
}