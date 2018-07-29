using System;
using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Lightmap.Editor {
	public class LightmapManager : MonoBehaviour {
		[Serializable]
		public class Snapshot {
			public ReflectionProbeSnapshot[] reflectionProbes; // specified from `managedReflectionProbes`
			public LightSnapshot[] lights; // specified from `managedLights`
			public RendererSnapshot[] renderers; // specified from `managedRenderers`
			public LightmapDataSnapshot[] textures; //automatically generated
			public LightProbes lightProbes; //automatically detected & generated
			public Cubemap skyboxReflection; //automatically generated
		}

		// parameters
		public LightmapParameters defaultParameters;
		public LightmapParameters transparentParameters;
		public Material transparentMaterial;

		// scene objects
		public Renderer[] managedRenderers;
		public Light[] managedLights;
		public ReflectionProbe[] managedReflectionProbes;

		// snapshots
		public Snapshot[] snapshots;
	}
}
