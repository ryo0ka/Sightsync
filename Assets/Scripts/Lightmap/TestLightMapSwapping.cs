using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TestLightMapSwapping : MonoBehaviour {
	// class that represents a state of lightmap
	[Serializable]
	public class LightmapVersion {
		// Swaps the 2nd UV of baked meshes in the scene.
		public Texture2D texture;
			
		// Swaps LightProbes in the scene.
		// NOTE: Create a LightProbes asset for this property via the Editor script.
		public LightProbes lightProbes;

		// Swaps Default reflection of the scene.
		public Cubemap skybox;

		// Swaps texture of TestLightMapSwapping::reflectionProbe.
		public Texture reflectionProbeTexture;

		// Swaps rotation of TestLightMapSwapping::environmentLight.
		public Vector3 environmentLightRotation;
	}

	// the main Light object in the scene.
	// LightmapVersion::environmentLightRotation affects this.
	public Light environmentLight;

	// a tested ReflectionProbe object in the scene.
	// LightmapVersion::reflectionProbeTexture affects this.
	public ReflectionProbe reflectionProbe;

	// a slider to switch entries.
	public Slider slider;

	// a list of entries.
	public LightmapVersion[] versions;

	void Start() {
		// Set up slider
		slider.wholeNumbers = true;
		slider.minValue = 0;
		slider.maxValue = versions.Length - 1;
		slider.onValueChanged.AddListener(i => SwapLightmap((int)i));

		// initialize the lightmap with the first entry
		SwapLightmap(0);
	}

	void SwapLightmap(int index) {
		// Swap lightmap texture
		Texture2D lightmapTexture = versions[index].texture;
		LightmapData data = new LightmapData { lightmapColor = lightmapTexture };
		LightmapSettings.lightmaps = new LightmapData[] { data };

		// Swap LightProbes
		LightProbes lightProbes = versions[index].lightProbes;
		LightmapSettings.lightProbes = lightProbes;

		// Swap environment reflection
		Cubemap source = versions[index].skybox;
		RenderSettings.customReflection = source;
		RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;

		// Swap Reflection probes
		Texture texture = versions[index].reflectionProbeTexture;
		reflectionProbe.bakedTexture = texture;

		// Swap Environment Light rotation
		Vector3 rotation = versions[index].environmentLightRotation;
		environmentLight.transform.eulerAngles = rotation;
	}
}
