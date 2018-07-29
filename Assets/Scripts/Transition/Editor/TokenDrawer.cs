using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Transition.Editor {
	[CustomPropertyDrawer(typeof(DateTransitionManager.Token))]
	public class TokenDrawer : PropertyDrawer {
		SerializedProperty managerProperty;
		SerializedProperty entryNameProperty;
		DateTransitionManager manager;
		string[] entryNames;
		string[] entryNamesDecorated;
		int selectedEntryIndex;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			//// Sort out the state

			managerProperty = property.FindPropertyRelative("manager");
			entryNameProperty = property.FindPropertyRelative("entryName");
			manager = managerProperty.objectReferenceValue as DateTransitionManager;

			// try finding an existing manager in the scene
			if (manager == null) {
				manager = GameObject.FindObjectOfType<DateTransitionManager>();
				managerProperty.objectReferenceValue = manager;
			}
			
			if (manager != null) {
				entryNames = manager.entries.Select(e => e.name).ToArray();

				// Use this for the popup's item list instead
				entryNamesDecorated = manager.entries.Select(
					e => e.name + " (" + e.yearAppeared + " ~ " + e.yearDisappeared + ")")
					.ToArray();

				selectedEntryIndex = ArrayUtility.IndexOf(entryNames, entryNameProperty.stringValue);
				
				if (selectedEntryIndex < 0) {
					selectedEntryIndex = 0;
				}
			}

			//// Draw the state & receive user input

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			EditorGUILayout.PropertyField(managerProperty);

			if (manager == null) {
				EditorGUILayout.HelpBox("Manager doesn't exist in the scene.", MessageType.Warning);
			} else {
				selectedEntryIndex = EditorGUILayout.Popup("Entry Name", selectedEntryIndex, entryNamesDecorated);

				entryNameProperty.stringValue = entryNames[selectedEntryIndex];
			}

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;
		}
	}
}
