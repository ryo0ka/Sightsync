using UnityEngine;
using UnityEditor;

namespace Assets.Ryooka.Scripts.EditorExtension.Editor {
	[CustomPropertyDrawer(typeof(UnfoldInInspector))]
	class UnfoldInInspectorEditor: PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.indentLevel -= 1;
			property.isExpanded = true;
			position.y -= EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(position, property, GUIContent.none, true);
			EditorGUI.indentLevel += 1;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			float height = EditorGUI.GetPropertyHeight(property, GUIContent.none, true);
			return Mathf.Max(0, height - EditorGUIUtility.singleLineHeight);
		}
	}
}
