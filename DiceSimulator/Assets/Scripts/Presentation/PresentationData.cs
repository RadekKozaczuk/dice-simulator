#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Core;
using Presentation.Views;

namespace Presentation
{
    /// <summary>
    /// Assembly-level data.
    /// </summary>
    static class PresentationData
    {
        internal static readonly Dictionary<int, DiceView> Balls = new();
        internal static readonly Dictionary<int, BrickView> Bricks = new();

        /// <summary>
        /// Not every brick will have a hp label.
        /// </summary>
        internal static readonly Dictionary<int, HpView> HpLabels = new();

        /// <summary>
        /// Key is the scene build id.
        /// </summary>
        internal static readonly Dictionary<Level, LevelSceneReferenceHolder> SceneReferenceHolders = new();
    }
}