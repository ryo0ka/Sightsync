using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestLightMapParametersSwapping))]
public class TestLightMapParametersSwappingEditor : UnityEditor.Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		//if (GUILayout.Button("Make Materials Transparent")) {
		//	TestLightMapParametersSwapping instance = target as TestLightMapParametersSwapping;
		//	Renderer renderer = instance.GetComponent<Renderer>();
		//	Material transparentMaterial = instance.transparentMaterial;

		//	Material[] originalMaterials = renderer.sharedMaterials.ToArray();
		//	Material[] destinationMaterials = new Material[originalMaterials.Length];

		//	for (int i = 0; i < originalMaterials.Length; i++) {
		//		destinationMaterials[i] = transparentMaterial;
		//	}

		//	renderer.sharedMaterials = destinationMaterials;
		//}
		
		//if (GUILayout.Button("Turn on IsTransparent")) {
		//	TestLightMapParametersSwapping instance = target as TestLightMapParametersSwapping;
		//	SerializedObject instanceSO = new SerializedObject(instance); // also can be `serializedObject`
		//	SerializedProperty isTransparentSP = instanceSO.FindProperty("isTransparent");
		//	bool isTransparent = isTransparentSP.boolValue;

		//	isTransparentSP.boolValue = true;
		//	instanceSO.ApplyModifiedProperties(); // Apply the change!
		//}

		//if (GUILayout.Button("Make Parameters Transparent")) {
		//	TestLightMapParametersSwapping instance = target as TestLightMapParametersSwapping;
		//	LightmapParameters transparentParam = instance.transparentParameters;
		//	SerializedObject rendererSO = new SerializedObject(instance.GetComponent<Renderer>());
		//	SerializedProperty paramSP = rendererSO.FindProperty("m_LightmapParameters");
		//	LightmapParameters originalParam = paramSP.objectReferenceValue as LightmapParameters;

		//	paramSP.objectReferenceValue = transparentParam;
		//	rendererSO.ApplyModifiedProperties();
		//}

		//if (GUILayout.Button("Test Bake OnComplete")) {
		//	Lightmapping.completed += () => { Debug.Log("Foo"); }; // this won't work if terminated
		//	Debug.Log(Lightmapping.BakeAsync()); // BakeAsync() always returns true right away
		//	// Bake() might return false when terminated, but can't find a way to terminate it...
		//	// isRunning() might be useful to determine when terminated?
		//}
	}
}
