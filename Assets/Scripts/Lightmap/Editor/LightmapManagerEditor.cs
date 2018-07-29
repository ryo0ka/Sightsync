using System;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace Assets.Scripts.Lightmap.Editor {
	[CustomEditor(typeof(LightmapManager))]
	class LightmapManagerEditor : UnityEditor.Editor {
		SerializedProperty renderers;

		RendererSnapshotList renderersList;
		ReorderableList lightsList;
		ReorderableList reflectionProbesList;

		private void OnEnable() {
			renderers = serializedObject.FindProperty("managedRenderers");
			renderersList = new RendererSnapshotList(serializedObject, renderers);

			lightsList = new ReorderableList(
				serializedObject: serializedObject,
				elements: serializedObject.FindProperty("managedLights"));

			reflectionProbesList = new ReorderableList(
				serializedObject: serializedObject,
				elements: serializedObject.FindProperty("managedReflectionProbes"));
		}

		public override void OnInspectorGUI() {
			// parameters
			EditorGUILayout.ObjectField(serializedObject.FindProperty("defaultParameters"));
			EditorGUILayout.ObjectField(serializedObject.FindProperty("transparentParameters"));
			EditorGUILayout.ObjectField(serializedObject.FindProperty("transparentMaterial"));

			// scene objects
			renderersList.DoLayoutList();
			lightsList.DoLayoutList();
			reflectionProbesList.DoLayoutList();

			serializedObject.ApplyModifiedProperties();
		}

		class RendererSnapshotList : ReorderableList {
			public RendererSnapshotList(SerializedObject so, SerializedProperty rs) : base(so, rs) {
				displayAdd = true;
				displayRemove = true;
				draggable = false;
				elementHeight = EditorGUIUtility.singleLineHeight;

				drawHeaderCallback += (rect) => {
					GUI.Label(rect, "Renderers");
				};

				drawElementCallback += (rect, index, active, focused) => {
					EditorGUI.PropertyField(rect, rs.GetArrayElementAtIndex(index), GUIContent.none);

				};
			}
		}
	}
}
