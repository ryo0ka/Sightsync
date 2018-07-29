using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestLightMapSwapping))]
class TestLightMapSwappingEditor: UnityEditor.Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		if (GUILayout.Button("Generate LightProbes Asset")) {
			string path = GenerateLightProbesAsset();
			Debug.Log(path);
		}
	}

	string GenerateLightProbesAsset() {
		string dirPath = GetCurrentSceneFolderPath();
		string filePath = Path.Combine(dirPath, "lightprobe.asset");
			
		Directory.CreateDirectory(dirPath);
		CreateCurrentLightProbesAssetAt(filePath);
			
		return filePath;
	}

	// create a LightProbes asset of the current bake
	void CreateCurrentLightProbesAssetAt(string path) {
		LightProbes probes = LightmapSettings.lightProbes;
		AssetDatabase.CreateAsset(Instantiate(probes), path);
	}

	// i.e. Assets/Scenes/MyScene where MyScene.unity exists in Assets/Scenes.
	string GetCurrentSceneFolderPath() {
		string fullPath = EditorApplication.currentScene;
		string parent = Path.GetDirectoryName(fullPath);
		string name = Path.GetFileNameWithoutExtension(fullPath);
		return Path.Combine(parent, name);
	}
}
