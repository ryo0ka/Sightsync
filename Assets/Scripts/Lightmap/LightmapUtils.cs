using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Lightmap {
	public static class LightmapUtils {
		public static Texture2D[] ColorTextures {
			get {
				LightmapData[] dataSet = LightmapSettings.lightmaps;
				Texture2D[] textures = new Texture2D[dataSet.Length];

				for (int i = 0; i < textures.Length; i++) {
					textures[i] = dataSet[i].lightmapColor;
				}

				return textures;
			}

			set {
				LightmapData[] dataSet = LightmapSettings.lightmaps;

				// TODO prevent overflows when arrays have different length
				for (int i = 0; i < value.Length; i++) {
					LightmapData data = (dataSet.Length >= i)
						? dataSet[i]
						: new LightmapData();

					data.lightmapColor = value[i];
					dataSet[i] = data;
				}

				LightmapSettings.lightmaps = dataSet;
			}
		}

		public static LightProbes LightProbes {
			get {
				return LightmapSettings.lightProbes;
			}

			set {
				LightmapSettings.lightProbes = value;
			}
		}

		public static Cubemap DefaultReflection
		{
			get
			{
				return RenderSettings.customReflection;
			}

			set
			{
				RenderSettings.customReflection = value;
				RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
			}
		}

#if UNITY_EDITOR
		public static string GenerateLightProbesAsset(string assetName) {
			string fullPath   = EditorApplication.currentScene;
			string parentPath = Path.GetDirectoryName(fullPath);
			string fileName   = Path.GetFileNameWithoutExtension(fullPath);
			string dirPath    = Path.Combine(parentPath, fileName);
			string filePath   = Path.Combine(dirPath, assetName + ".asset");

			LightProbes currentLightProbes = LightProbes;

			try {
				Directory.CreateDirectory(dirPath);
				AssetDatabase.CreateAsset(
					asset: GameObject.Instantiate(currentLightProbes), 
					path: filePath);
			} catch (IOException e) {
				Debug.LogAssertion("Faild generating LightProbe asset: " + e);
			}

			return filePath;
		}

		public static LightmapParameters GetParameters(this Renderer renderer) {
			SerializedObject rendererSO = new SerializedObject(renderer);
			SerializedProperty parametersSP = rendererSO.FindProperty("m_LightmapParameters");
			return parametersSP.objectReferenceValue as LightmapParameters;
		}

		public static void SetParameters(this Renderer renderer, LightmapParameters dstParameters) {
			SerializedObject rendererSO = new SerializedObject(renderer);
			SerializedProperty parametersSP = rendererSO.FindProperty("m_LightmapParameters");
			
			parametersSP.objectReferenceValue = dstParameters;

			rendererSO.ApplyModifiedProperties();
		}
#endif
	}
}
