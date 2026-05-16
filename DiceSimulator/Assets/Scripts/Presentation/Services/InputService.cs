#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using GameLogic.ViewModels;
using Presentation.Config;
using Presentation.Popups;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Presentation.Services
{
    static class InputService
    {
        const string Quit = "Quit";
        const string MousePosition = "MousePosition";
        const string Shot = "Shot";

        static readonly UIConfig _uiConfig;

        static InputAction _movementAction;
        static InputAction _mousePositionAction;
        static InputAction _shotAction;

        internal static void Initialize()
        {
            // MainMenu bindings
            InputActionMap mainMenu = _uiConfig.InputActionAsset.FindActionMap(Constants.MainMenuActionMap);
            mainMenu.FindAction(Quit).performed += static _ =>
            {
                // if there is a popup - close it
                // otherwise quit the game
                if (PopupService.CurrentPopup == null)
                {
                    GameLogicViewModel.QuitGame();

#if UNITY_EDITOR
                    UnityEditor.EditorApplication.ExitPlaymode();
#else
                    Application.Quit();
#endif
                }
                else
                    PopupService.CloseCurrentPopup();
            };

            // Gameplay bindings
            InputActionMap gameplay = _uiConfig.InputActionAsset.FindActionMap(Constants.GameplayActionMap);
            gameplay.FindAction(Quit).performed += static _ => PopupService.ShowPopup(PopupType.QuitGame);

            _mousePositionAction = gameplay.FindAction(MousePosition);

            _shotAction = gameplay.FindAction(Shot);
            _shotAction.performed += static _ => GameLogicViewModel.MouseClickPosition = _mousePositionAction.ReadValue<Vector2>();

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