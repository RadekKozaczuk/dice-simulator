#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Core
{
    public enum GameState
    {
        Boot,
        MainMenu,
        Gameplay
    }

    public enum Sound
    {
        ClickHit
    }

    // todo: kinda misleading name if it also consist boot, core, and alike
    public enum Level
    {
        BootScene = 0,
        CoreScene = 1,
        MainMenuScene = 2,
        UIScene = 3,
        LevelScene = 4
    }

    public enum Music
    {
        MainMenu
    }

    public enum PopupType
    {
        QuitGame,

        /// <summary>
        /// Settings accessible from the main menu.
        /// </summary>
        Settings,
        LeaderBoard
    }

    /// <summary>
    /// For clarity, it should match physical layers.
    /// </summary>
    public enum CollisionEntityType
    {
        Undefined = int.MinValue,
        Ball = 0,
        Brick = 1,
        DestructionArea = 2
    }

    public enum BrickType
    {
        Basic,
        Bomb
    }
}