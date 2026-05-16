#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Core;
using Core.Services;
using Presentation.ViewModels;
using Presentation.Views;
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

            // destroy hp labels
            foreach (KeyValuePair<int, HpView> kvp in PresentationData.HpLabels)
                Destroy(kvp.Value.gameObject);
            PresentationData.HpLabels.Clear();

            GameStateService.ChangeState(GameState.MainMenu);
        }
    }
}