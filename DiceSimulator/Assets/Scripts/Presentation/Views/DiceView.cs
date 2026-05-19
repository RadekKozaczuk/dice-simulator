using System.Collections.Generic;
using Core.Views;
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
