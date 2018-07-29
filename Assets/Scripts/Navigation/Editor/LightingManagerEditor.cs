using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Navigation.Editor {
	[CustomEditor(typeof(LightingManager))]
	public class LightingManagerEditor : UnityEditor.Editor {
		LightingManager manager;
		Camera editorCamera;

		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			manager = (LightingManager)target;
			editorCamera = SceneView.GetAllSceneCameras()[0];

			if (!Application.isPlaying && GUILayout.Button("Update View")) {
				manager.UpdateLightsWithEditorCamera();
			}

			if (!Application.isPlaying && GUILayout.Button("Activate All")) {
				manager.ActivateAllLights();
			}

			if (!Application.isPlaying && GUILayout.Button("Bake Lightmap")) {
				manager.Bake();
			}
		}
	}
}
