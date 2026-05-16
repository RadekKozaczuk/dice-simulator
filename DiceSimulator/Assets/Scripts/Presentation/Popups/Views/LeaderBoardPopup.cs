#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Core;
using Core.Services;
using Presentation.Config;
using Presentation.ViewModels;
using Presentation.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Popups.Views
{
    [DisallowMultipleComponent]
    class LeaderBoardPopup : AbstractPopup
    {
        [SerializeField]
        Button _leave;

        [SerializeField]
        RectTransform _list;

        static readonly UIConfig _config;

        LeaderBoardPopup() : base(PopupType.LeaderBoard) { }

        internal override void Initialize()
        {
            base.Initialize();
            _leave.onClick.AddListener(LeaveAction);

            SetValues("Radek", CoreData.Score);
            SetValues("Exgs", 5);
            SetValues("MFisdmfs", 10);
            SetValues("fMIASD", 15);
        }

        internal override void Close() { }

        internal void SetValues(string playerName, int score)
        {
            LeaderBoardElementView view = Instantiate(_config.LeaderBoardElementView, _list.transform);
            view.Initialize(playerName, score);
        }

        static void LeaveAction()
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