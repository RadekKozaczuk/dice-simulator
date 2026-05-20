using System;
using System.Collections.Generic;
using Core;
using Core.Services;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Boot
{
    /// <summary>
    /// Contains all the high-level logic that cannot be executed from within <see cref="GameLogic" /> namespace.
    /// </summary>
    [DisallowMultipleComponent]
    class BootView : MonoBehaviour
    {
        [SerializeField]
        EventSystem _eventSystem;

        [ReadOnly]
        [SerializeField]
        [InfoBox("This field is automatically set on scene load.", InfoMessageType.None)]
        List<ScriptableObject> _configs;

        static bool _isCoreSceneLoaded;
        static GameStateMachine<GameState> _gameStateMachine;

        void Awake()
        {
#if UNITY_EDITOR
            // this is to prevent edge cases when we open the editor or run the game from a different scene than BootScene.
            // in such cases logic present in BootSceneStartService will not inject the configs resulting in list being empty.
            // to avoid that we check for that and fill them up
            if (_configs.Count == 0)
                _configs = ArchitectureService.GetConfigs();
#endif

            // increase priority so that main menu can appear faster
            Application.backgroundLoadingPriority = ThreadPriority.High;

            // injection must be done in awake because fields cannot be injected into in the same method they are used in
            // start will be at least 1 frame later than Awake.
            ArchitectureService.Initialize(
                SignalProcessorPrecalculatedArrays.SignalCount,
                SignalProcessorPrecalculatedArrays.SignalNames,
                SignalProcessorPrecalculatedArrays.SignalQueues,
                _configs);
        }

        void Start()
        {
            SceneManager.sceneLoaded += static (scene, _) =>
            {
                if (scene.buildIndex == (int)SceneId.CoreScene)
                {
                    SceneManager.UnloadSceneAsync((int)SceneId.BootScene);
                    _isCoreSceneLoaded = true;

                    PresentationViewModel.OnCoreSceneLoaded();
                }
            };

            _gameStateMachine = CreateStateMachine();

            GameStateService.OnChangeState += (state, scenesToLoad, scenesToUnload) =>
                _ = _gameStateMachine.ChangeState(state, scenesToLoad, scenesToUnload);

            GameStateService.OnGetCurrentGameState += _gameStateMachine.GetCurrentState;
            GameStateService.ChangeState(GameState.MainMenu);

            DontDestroyOnLoad(_eventSystem);
            DontDestroyOnLoad(this);

            Application.targetFrameRate = 60;
        }

        void Update()
        {
            if (GameStateService.CurrentState == GameState.Boot)
                return;

            if (_isCoreSceneLoaded)
                ArchitectureService.ExecuteSentSignals();
        }

        GameStateMachine<GameState> CreateStateMachine() =>
            new(new List<(GameState from, GameState to, Func<(int[], int[])> scenesToLoadUnload)>
                {
                    (GameState.Boot,
                     GameState.MainMenu,
                     static () => (new[] { (int)SceneId.MainMenuScene, (int)SceneId.CoreScene, (int)SceneId.UIScene },
                                   Array.Empty<int>())),
                    (GameState.MainMenu,
                     GameState.Gameplay,
                     static () => (new[] { (int)SceneId.LevelScene }, new[] { (int)SceneId.MainMenuScene })),
                    (GameState.Gameplay,
                     GameState.MainMenu,
                     static () => (new[] { (int)SceneId.MainMenuScene }, ScenesToUnloadFromGameplayToMainMenu()))
                },
                new (GameState, Action, Action)[]
                {
                    (GameState.Boot, static () => { }, static () => { }),
                    (GameState.MainMenu, MainMenuOnEntry, MainMenuOnExit),
                    (GameState.Gameplay, GameplayOnEntry, GameplayOnExit)
                }
            );

        static void MainMenuOnEntry()
        {
            GameLogicViewModel.MainMenuOnEntry();
            PresentationViewModel.MainMenuOnEntry();
        }

        static void MainMenuOnExit()
        {
            GameLogicViewModel.MainMenuOnExit();
            PresentationViewModel.MainMenuOnExit();
        }

        static void GameplayOnEntry()
        {
            GameLogicViewModel.GameplayOnEntry();
            PresentationViewModel.GameplayOnEntry();
        }

        static void GameplayOnExit()
        {
            GameLogicViewModel.GameplayOnExit();
            PresentationViewModel.GameplayOnExit();
        }

        /// <summary>
        /// Returns ids of all currently open scenes except for <see cref="SceneId.CoreScene" />,
        /// <see cref="SceneId.MainMenuScene" /> and <see cref="SceneId.UIScene" />
        /// </summary>
        static int[] ScenesToUnloadFromGameplayToMainMenu()
        {
            int countLoaded = SceneManager.sceneCount;
            var scenesToUnload = new List<int>(countLoaded);

            for (int i = 0; i < countLoaded; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if ((SceneId)scene.buildIndex is SceneId.CoreScene or SceneId.MainMenuScene or SceneId.UIScene)
                    continue;

                scenesToUnload.Add(scene.buildIndex);
            }

            return scenesToUnload.ToArray();
        }
    }
}