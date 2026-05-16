#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using UnityEngine.Assertions;

namespace Core
{
    /// <summary>
    /// Application-level (global) data.
    /// All static data objects used across the project are stored here.
    /// </summary>
    public static class CoreData
    {
        public static int Score // todo: move to CoreData
        {
            get => _score;
            set
            {
                Assert.IsTrue(value >= 0, "Score must be greater than or equal to 0");
                _score = value;
                Signals.ScoreChanged();
            }
        }
        static int _score;
    }
}