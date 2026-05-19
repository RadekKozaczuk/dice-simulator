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

    public enum Scene
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
        Settings
    }
}