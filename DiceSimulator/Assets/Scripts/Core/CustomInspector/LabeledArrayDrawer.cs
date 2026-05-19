#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

namespace Core.CustomInspector
{
    /// <summary>
    /// This class always us to display enum's name next to the value in the inspector
    /// This is used for example: numbers or drop-in thing like prefabs or scriptable objects
    /// </summary>
    [CustomPropertyDrawer(typeof(LabeledArrayAttribute))]
    public class LabeledArrayDrawer<T> : OdinAttributeDrawer<LabeledArrayAttribute, T>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            int index = Property.Index;
            // If label is null, it means it's element of array, otherwise it's the property itself
            if (label == null)
            {
                string name = index >= 0 && index < Attribute.Names.Length
                    ? Attribute.Names[index]
                    : "Unknow";

                CallNextDrawer(new GUIContent(name));
            }
            else
                CallNextDrawer(label);
        }
    }
}
#endif