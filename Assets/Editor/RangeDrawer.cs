using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Range))]
public class RangeDrawer : PropertyDrawer {
	
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		label = EditorGUI.BeginProperty(position, label, property);
		Rect contentPosition = EditorGUI.PrefixLabel(position, label);

		if (position.height > 16f) {
			position.height = 16f;
			EditorGUI.indentLevel += 1;
			contentPosition = EditorGUI.IndentedRect(position);
			contentPosition.y += 18f;
		}

		EditorGUI.indentLevel = 0;

		contentPosition.width /= 4f;
		EditorGUIUtility.labelWidth = 14f;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("worst"), GUIContent.none);
		contentPosition.x += contentPosition.width;

		contentPosition.width += 10f;
		EditorGUIUtility.labelWidth = 10f;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("best"), new GUIContent("-"));
		contentPosition.x += contentPosition.width;

		contentPosition.width += 4f;
		EditorGUIUtility.labelWidth = 14f;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("step"), new GUIContent("in"));

		EditorGUI.EndProperty();
	}

}