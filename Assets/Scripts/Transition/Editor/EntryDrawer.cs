using System;
using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Transition.Editor {
	[CustomPropertyDrawer(typeof(DateTransitionManager.Entry))]
	public class EntryDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Rect nameRect = position;
			nameRect.height = EditorGUIUtility.singleLineHeight;

			Rect yearAppearRect = nameRect;
			yearAppearRect.y += nameRect.height + 1;
			yearAppearRect.width /= 2;

			Rect yearDisappearRect = yearAppearRect;
			yearDisappearRect.x += yearDisappearRect.width;

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

			EditorGUI.PropertyField(yearAppearRect, property.FindPropertyRelative("yearAppeared"), GUIContent.none);

			EditorGUI.PropertyField(yearDisappearRect, property.FindPropertyRelative("yearDisappeared"), GUIContent.none);

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return EditorGUIUtility.singleLineHeight * 2;
		}
	}
}
