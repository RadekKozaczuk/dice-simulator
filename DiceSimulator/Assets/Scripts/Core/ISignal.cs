using UnityEngine;

// ReSharper disable UnusedMemberInSuper.Global

namespace Core
{
    public interface ISignal
    {
        void DicePositionChanged(Vector3 position, Quaternion rotation);

        void DiceStopped();

        /// <summary>
        /// Hp after the hit.
        /// </summary>
        void BrickHit(int id, int currentHp);

        /// <summary>
        /// Position is x and z.
        /// </summary>
        void BrickSpawned(int id, BrickType brickType, Vector2 position, float rotation, float scale, int hp);

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