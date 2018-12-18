using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


// Taken from: http://answers.unity.com/answers/801283/view.html
// NOTE: Doesn't work well with fields that already use a custom property drawer


/// <summary>
/// Custom drawer that supports readonly attributes
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
