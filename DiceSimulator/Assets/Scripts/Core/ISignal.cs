using UnityEngine;

// ReSharper disable UnusedMemberInSuper.Global

namespace Core
{
    public interface ISignal
    {
        void DicePositionChanged(Vector3 position, Quaternion rotation);

        void DiceStopped();

        void DiceSpawned(Vector3 position, Quaternion rotation);

        /// <summary>
        /// Sent after the last ball leaves the map.
        /// </summary>
        void GameEnded();

        /// <summary>
        /// Indicates that the score has changed.
        /// Score can be read from <see cref="CoreData.Score"/>
        /// </summary>
        void ScoreChanged();
    }
}