#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using Core.Services;
using Presentation.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Popups.Views
{
    [DisallowMultipleComponent]
    class QuitGamePopup : AbstractPopup
    {
        [SerializeField]
        Button _mainMenu;

        QuitGamePopup() : base(PopupType.QuitGame) { }

        internal override void Initialize()
        {
            base.Initialize();
            _mainMenu.onClick.AddListener(MainMenuAction);
        }

        static void MainMenuAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickHit);
            PopupService.CloseCurrentPopup();
            GameStateService.ChangeState(GameState.MainMenu);
        }
    }
}