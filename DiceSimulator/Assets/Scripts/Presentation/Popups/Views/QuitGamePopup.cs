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