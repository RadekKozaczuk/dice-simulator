#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Services;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameStateMachine<TState> : GameStateMachineInternal<TState>
        where TState : struct, Enum
    {
        // used to remember what state preload wanted to go
        // null otherwise
        TransitionDto? _transition;

        /// <summary>
        /// 'betweenLoadAndUnload' action is the best suitable for scenarios when we need to just when scenes stopped loading but right before they start to unload.
        /// Great example would be when we go from a level to a level and the level we are leaving is going to disappear.
        /// </summary>
        public GameStateMachine(
            IReadOnlyList<(TState from, TState to, Func<(int[]?, int[]?)>? scenesToLoadUnload)> transitions,
            IReadOnlyList<(TState state, Action? onEntry, Action? onExit)> states)
            : base(transitions, states) { }

        /// <summary>
        /// Actual state change may be delayed in time. Consecutive calls are not allowed.
        /// Additional scenes, whether to-load or to-unload, must not collide with the scenes defined in the constructor.
        /// </summary>
        /// <param name="state">State we transition to</param>
        /// <param name="additionalScenesToLoad">Additional scenes (not defined in the transition) to load during</param>
        /// <param name="additionalScenesToUnload"></param>
        /// <exception cref="Exception"></exception>
        public async Awaitable ChangeState(TState state, int[]? additionalScenesToLoad = null, int[]? additionalScenesToUnload = null)
        {
            List<TransitionDto> transitions = _transitions.FindAll(t => Equal(t.From, _currentState) && Equal(t.To, state));

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (transitions.Count > 1)
                throw new Exception($"Transition from {_currentState} to {state} is defined more than once.");
            if (transitions.Count == 0)
                throw new Exception($"Transition from {_currentState} to {state} is not defined.");
#endif

            TransitionDto transition = transitions[0];
            (int[]? scenesToLoad, int[]? scenesToUnload)? scenesToLoadUnload = transition.ScenesToLoadUnload?.Invoke();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (scenesToLoadUnload != null)
            {
                Assert.IsFalse(Utils.HasDuplicates(CombineArrays(scenesToLoadUnload.Value.scenesToLoad, additionalScenesToLoad)),
                    "GameStateMachine was asked to load the same scene more than once.");
                Assert.IsFalse(Utils.HasDuplicates(CombineArrays(scenesToLoadUnload.Value.scenesToUnload, additionalScenesToUnload)),
                    "GameStateMachine was asked to unload the same scene more than once.");
            }
#endif

            // execute state's on-exit code
            _states.TryGetValue(transition.From, out StateDto fromState);
            fromState.OnExit?.Invoke();

            if (scenesToLoadUnload != null)
                if (scenesToLoadUnload.Value.scenesToLoad is { Length: > 0 } || additionalScenesToLoad is { Length: > 0 })
                    await LoadScenes(CombineArrays(scenesToLoadUnload.Value.scenesToLoad, additionalScenesToLoad));

            // change state
            _currentState = state;

            // execute state's on-entry code
            _states.TryGetValue(transition.To, out StateDto toState);

            if (scenesToLoadUnload != null)
                if (scenesToLoadUnload.Value.scenesToUnload is { Length: > 0 } || additionalScenesToUnload is { Length: > 0 })
                    UnloadScenes(CombineArrays(scenesToLoadUnload.Value.scenesToUnload, additionalScenesToUnload));

            // actual end of the transition
            toState.OnEntry?.Invoke();
        }

        /// <summary>
        /// Scenes are loaded normally and all at once.
        /// </summary>
        static async Awaitable LoadScenes(params int[] scenes)
        {
            var asyncOperations = new AsyncOperation[scenes.Length];
            asyncOperations[0] = SceneManager.LoadSceneAsync(scenes[0], LoadSceneMode.Additive)!;

            for (int i = 1; i < scenes.Length; i++)
                asyncOperations[i] = SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive)!;

            await AwaitAsyncOperations(asyncOperations);

            // wait a frame so every Awake and Start method is called
            await Awaitable.NextFrameAsync();
        }

        static void UnloadScenes(params int[] scenes)
        {
            // unload scenes shoot and forger
            foreach (int scene in scenes)
                SceneManager.UnloadSceneAsync(scene);
        }

        /// <summary>
        /// Waits until all operations are either done or have progress greater equal 0.9.
        /// </summary>
        static async Awaitable AwaitAsyncOperations(params AsyncOperation[] operations)
        {
            while (!operations.All(static t => t.isDone))
                await Awaitable.NextFrameAsync();
        }
    }
}