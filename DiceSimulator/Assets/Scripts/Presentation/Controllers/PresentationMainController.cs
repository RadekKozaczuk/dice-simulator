using System.Collections.Generic;
using Core;
using Core.Dtos;
using JetBrains.Annotations;
using Presentation.Config;
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
    class PresentationMainController
    {
        static readonly PresentationConfig _presentationConfig;
        static readonly UIConfig _uiConfig;

        static DiceView _dice;

        [Preserve]
        PresentationMainController() { }

        internal static void OnCoreSceneLoaded()
        {
            SoundService.Initialize();
            MusicService.Initialize();
        }

        [React]
        static void OnDiceSpawned(Vector3 position, Quaternion rotation, List<DiceFace> faces)
        {
            _dice = Object.Instantiate(_presentationConfig.DicePrefab, position, rotation);
            _dice.gameObject.name = "Dice";
            _dice.SpawnFaces(faces);
        }

        [React]
        static void OnDiceStopped(int result, int total)
        {
            PanelView panel = UISceneReferenceHolder.Panel;
            panel.RollEnded(result, total);
        }

        [React]
        static void OnDicePositionChanged(Vector3 position, Quaternion rotation) =>
            _dice.gameObject.transform.SetPositionAndRotation(position, rotation);
    }
}