namespace Shared.Systems;

public static class Architecture
{
    public static T Interception<T>(T signals) where T : class => signals;
}