#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using UnityEngine;

namespace GameLogic.Dtos
{
    /// <summary>
    /// BallDto is used in the presentation layer to reflect player data.
    /// Properties with custom setters (backed by private fields) are used when updates should trigger UI or game signals.
    /// Simple public fields are used when data does not require such propagation.
    /// </summary>
    class BallDto
    {
        internal Vector2 Position
        {
            set
            {
                if (value == _position)
                    return;
                
                _position = value;
                Signals.BallPositionChanged(_id, _position);
            }
        }
        Vector2 _position;

        readonly int _id;

        internal BallDto(int id, Vector2 position)
        {
            _id = id;
            _position = position;
            Signals.BallSpawned(id, _position);
        }
    }
}