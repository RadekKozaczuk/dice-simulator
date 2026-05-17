#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
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

        static readonly BallConfig _ballConfig;
        static readonly DiceConfig _diceConfig;
        static readonly UIConfig _uiConfig;

        static int _canvasWidth;
        static int _canvasHeight;

        [Preserve]
        PresentationMainController() { }

        public void CustomUpdate()
        {
            if (!_coreSceneLoaded)
                return;

            foreach (KeyValuePair<int, DiceView> kvp in PresentationData.Balls)
                kvp.Value.CustomUpdate();
        }

        internal static void OnCoreSceneLoaded()
        {
            SoundService.Initialize();
            MusicService.Initialize();
            _coreSceneLoaded = true;
        }

        [React]
        static void OnBallDestroyed(int id)
        {
            Object.Destroy(PresentationData.Balls[id].gameObject);
            PresentationData.Balls.Remove(id);
        }

        [React]
        static void OnBallPositionChanged(int id, Vector2 position) =>
            PresentationData.Balls[id].transform.position = new Vector3(position.x, 0, position.y);

        [React]
        static void OnBallsLeftChanged(int currentCount) =>
            UISceneReferenceHolder.BallsLeft.SetValue(currentCount);

        [React]
        static void OnBallSpawned(int id, Vector2 position)
        {
            LevelSceneReferenceHolder holder = PresentationData.SceneReferenceHolders[Level.LevelScene];
            var pos = new Vector3(position.x, 0, position.y);
            DiceView dice = Object.Instantiate(_ballConfig.Prefab, pos, Quaternion.identity, holder.BallsContainer);
            PresentationData.Balls.Add(id, dice);
        }

        [React]
        static void OnBrickDestroyed(int id)
        {
            Object.Destroy(PresentationData.Bricks[id].gameObject);
            PresentationData.Bricks.Remove(id);

            Object.Destroy(PresentationData.HpLabels[id].gameObject);
            PresentationData.HpLabels.Remove(id);
        }

        [React]
        static void OnBrickHit(int id, int currentHp)
        {
            if (PresentationData.HpLabels.TryGetValue(id, out HpView hpLabel))
                hpLabel.Hp = currentHp;

            SoundService.Play(Sound.ClickHit);
        }

        [React]
        static void OnBrickSpawned(int id, BrickType brickType, Vector2 position, float rotation, float scale, int hp)
        {
            LevelSceneReferenceHolder holder = PresentationData.SceneReferenceHolders[Level.LevelScene];
            var pos = new Vector3(position.x, 0, position.y);
            var rot = Quaternion.Euler(0, rotation, 0);

            switch (brickType)
            {
                case BrickType.Basic:
                {
                    DiceView brick = Object.Instantiate(_diceConfig.Dice, pos, rot, holder.BricksContainer);
                    brick.transform.localScale = new Vector3(scale, 1, 1);
                    //PresentationData.Bricks.Add(id, brick);
                    break;
                }
                case BrickType.Bomb:
                    throw new NotImplementedException("Bomb will be added in a DLC");
                default:
                    throw new ArgumentOutOfRangeException(nameof(brickType), brickType, null);
            }

            // undestructable bricks should not have the hp label
            if (hp == int.MinValue)
                return;

            // spawn hp view - but only for element with non-zero hp
            var canPos = new Vector3(
                position.x + Core.Constants.MapSizeX / 2,
                position.y + Core.Constants.MapSizeY / 2,
                0);

            Camera cam = PresentationSceneReferenceHolder.GameplayCamera;
            int height = cam.pixelHeight;
            int width = cam.pixelWidth;
            canPos.x = canPos.x * width / Core.Constants.MapSizeX;
            canPos.y = canPos.y * height / Core.Constants.MapSizeY;

            Transform parent = UISceneReferenceHolder.HpLabelContainer.transform;
            HpView view = Object.Instantiate(_uiConfig.HpLabel, canPos, Quaternion.identity, parent);
            view.Hp = hp;
            PresentationData.HpLabels.Add(id, view);
        }

        [React]
        static void OnGameEnded() => PopupService.ShowPopup(PopupType.LeaderBoard);

        [React]
        static void OnScoreChanged()
        {
            //UISceneReferenceHolder.Score.SetValue(CoreData.Score);
        }

        [React]
        static void OnDiceSpawned(Vector3 position, Quaternion rotation)
        {
            Debug.LogError("Dice spawned");
            DiceView view = Object.Instantiate(_diceConfig.Dice, position, rotation);
            view.gameObject.name = "Dice";
        }
    }
}