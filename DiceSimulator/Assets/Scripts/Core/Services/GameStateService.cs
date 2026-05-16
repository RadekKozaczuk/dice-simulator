#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

// ReSharper disable InvalidXmlDocComment

namespace Core.Services
{
    public delegate void ChangeState(GameState requested, int[]? additionalScenesToLoad = null, int[]? additionalScenesToUnload = null);
    public delegate GameState GetCurrentGameState();

    public static class GameStateService
    {
        public static event ChangeState OnChangeState;
        public static event GetCurrentGameState OnGetCurrentGameState;

        public static GameState CurrentState => OnGetCurrentGameState.Invoke();

        /// <summary>
        /// Scenes to load and unload are defined in <see cref="GameStateMachine{TState,TTransitionParameter}" />'s constructor.
        /// Additional scenes defined here are special cases that does not occur all the time and therefore could not be defined in the constructor.
        /// These scenes should not overlap with the ones defined in the GameStateMachine's constructor.
        /// Actual state change may be delayed in time. Consecutive calls are not allowed.
        /// </summary>
        public static void ChangeState(GameState state) => OnChangeState.Invoke(state);
    }
}