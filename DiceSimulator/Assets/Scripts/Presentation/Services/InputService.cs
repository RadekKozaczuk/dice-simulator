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
        static InputAction _rollAction;

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

            _rollAction = gameplay.FindAction(Shot);
            _rollAction.performed += static _ =>
            {
                Vector2 mousePosition = _mousePositionAction.ReadValue<Vector2>();
                GameLogicViewModel.MouseClickPosition = mousePosition;
                Debug.LogError($"GameLogicViewModel.MouseClickPosition: {mousePosition}");
            };

            _rollAction.canceled += static _ =>
            {
                Vector2 mousePosition = _mousePositionAction.ReadValue<Vector2>();
                GameLogicViewModel.MouseClickPosition = mousePosition;
                Debug.LogError($"Roll released: {mousePosition}");
                GameLogicViewModel.AutoRoll();
            };

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