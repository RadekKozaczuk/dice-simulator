namespace Common.Enums;

public enum Sound
{
    ClickSelect
}

public enum PopupType
{
    QuitGame,
    SigningIn
}

public static class ModifierEnums
{
    public enum StatModifierOperation
    {
        ChangMaxHp = 0,
        ChangeMaxArmor = 1,
        ChangeMovementSpeed = 2,
        ChangeAttackDamage = 3,
        ChangeAttackSpeed = 4,
        ChangeProjectileSpeed = 5,
        ChangeAttackRange = 6
    }
}