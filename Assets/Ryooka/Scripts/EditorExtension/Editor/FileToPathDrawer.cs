using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Ryooka.Scripts.EditorExtension.Editor {
	[CustomPropertyDrawer(typeof(FileToPath))]
	public class FileToPathDrawer: PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			label = EditorGUI.BeginProperty(position, label, property);
			
			if (property.propertyType != SerializedPropertyType.String) {
				//Displays warninig if assigned property is not of string.
				EditorGUI.LabelField(position, label.text, "Use FileToPath with string properties.");
			} else {
				bool dropped = false;
				if (position.Contains(Event.current.mousePosition)) {
					int id = GUIUtility.GetControlID(FocusType.Passive);
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					DragAndDrop.activeControlID = id;
					if (Event.current.type == EventType.DragPerform) {
						DragAndDrop.AcceptDrag();
						string value = property.stringValue;
                        GetName(DragAndDrop.objectReferences[0], ref value);
						property.stringValue = value;
						EditorGUI.PropertyField(position, property);
						DragAndDrop.activeControlID = 0; //Releases control of D&D.
						dropped = true;
					}
				}
				if (!dropped) {
					//Performs ordinally if D&D is not happening.
					EditorGUI.PropertyField(position, property);
				}
			}
			EditorGUI.EndProperty();
		}

		void GetName(UnityEngine.Object asset, ref string target) {
			string parentPath = (attribute as FileToPath).RootPath();
			string fullPath = AssetDatabase.GetAssetPath(asset);
			if (fullPath.StartsWith(parentPath)) {
				//subtracts the parent path and the separator.
				target = fullPath.Substring(parentPath.Length + 1);
			}
		}
	}
}
