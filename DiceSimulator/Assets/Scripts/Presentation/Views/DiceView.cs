#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using UnityEngine;

namespace Presentation.Views
{
    class DiceView : MonoBehaviour
    {
        [SerializeField]
        internal List<DiceFaceView> Faces;

        [SerializeField]
        Transform _model;
    }
}
