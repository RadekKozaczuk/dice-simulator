using System;
using System.Diagnostics;
using UnityEngine;

namespace Core.CustomInspector
{
    /// <summary>
    /// This class always allows us to get the enum related to the values we want to adjust, it is used when we draw the GUI
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    public class LabeledArrayAttribute : PropertyAttribute
    {
        public readonly string[] Names;

        public LabeledArrayAttribute(Type enumType) => Names = Enum.GetNames(enumType);
    }
}