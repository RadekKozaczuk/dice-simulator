#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using Presentation.Config;
using Presentation.Popups;
using Presentation.Services;
using Presentation.Views;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace Presentation.Controllers
{
    /// <summary>
    /// Main controller serves 3 distinct roles:<br/>
    /// 1) It allows you to control signal execution order. For example, instead of reacting on many signals in many different controllers,
    /// you can have one signal, react on it here, and call necessary controllers/systems in the order of your liking.<br/>
    /// 2) Serves as a 'default' controller. When you don't know where to put some logic or the logic is too small for its own controller
    /// you can put it into the main controller.<br/>
    /// 3) Reduces the size of the viewmodel. We could move all (late/fixed)update calls to viewmodel but over time it would lead to viewmodel
    /// being too long to comprehend. We also do not want to react on signals in viewmodels for the exact same reason.<br/>
    /// </summary>
    [UsedImplicitly]
    class PresentationMainController : ICustomUpdate
    {
        static bool _coreSceneLoaded;

        static readonly DiceConfig _diceConfig;
        static readonly UIConfig _uiConfig;

        static int _canvasWidth;
        static int _canvasHeight;

        static DiceView _dice;

        [Preserve]
        PresentationMainController() { }

        public void CustomUpdate()
        {
            //if (!_coreSceneLoaded)
            //    return;
        }

        internal static void OnCoreSceneLoaded()
        {
            SoundService.Initialize();
            MusicService.Initialize();
            _coreSceneLoaded = true;
        }

        [React]
        static void OnGameEnded() => PopupService.ShowPopup(PopupType.LeaderBoard);

        [React]
        static void OnDiceSpawned(Vector3 position, Quaternion rotation)
        {
            _dice = Object.Instantiate(_diceConfig.Dice, position, rotation);
            _dice.gameObject.name = "Dice";
        }

        [React]
        static void OnDiceStopped() => UISceneReferenceHolder.Panel.EnableRoll();

        [React]
        static void OnDicePositionChanged(Vector3 position, Quaternion rotation) =>
            _dice.gameObject.transform.SetPositionAndRotation(position, rotation);
    }
}