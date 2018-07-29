using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Navigation {
	public class LightingManager : MonoBehaviour {
		public abstract class Entity : MonoBehaviour {
			public abstract Collider GetCollider();

			public abstract IEnumerable<Light> PixelLights();

			public abstract IEnumerable<ReflectionProbe> ReflectionProbes();

			public void SetActive(bool? probeActive, bool? lightActive) {
				foreach (var probe in ReflectionProbes()) {
					if (probeActive.HasValue) {
						probe.enabled = probeActive.Value;
					}
				}

				foreach (var light in PixelLights()) {
					if (lightActive.HasValue) {
						light.enabled = lightActive.Value;
					}
				}
			}
		}

		public Collider player;

		public int editorPixelLightCount;

		public int runtimePixelLightCount;

		public bool emulatePixelLightCount;

		[SerializeField]
		[HideInInspector]
		List<Entity> entities;

		void OnValidate() {
			ApplyPixelLightCount();
		}

		void Awake() {
			ApplyPixelLightCount();
		}

		void Start() {
			foreach (var e in entities) {
				// Disable lights when the game starts.
				// (Will be enabled back if the player exists in the box.)
				e.SetActive(false, false);
			}
		}

		void ApplyPixelLightCount() {
			if (Application.isEditor && !emulatePixelLightCount) {
				QualitySettings.pixelLightCount = editorPixelLightCount;
			} else {
				QualitySettings.pixelLightCount = runtimePixelLightCount;
			}
		}

		public IEnumerable<Entity> Entities() {
			return entities.AsEnumerable();
		}

		public void Register(Entity entity) {
			if (!entities.Contains(entity)) {
				entities.Add(entity);
			}
		}

		public bool ConfirmPlayer(Collider collider) {
			return collider == player;
		}
		
		[ContextMenu("Activate All Lights")]
		public void ActivateAllLights() {
			foreach (LightingEntity e in Entities()) {
				e.SetActive(true, true);
			}
		}

#if UNITY_EDITOR
		[ContextMenu("Update Lights")]
		public void UpdateLightsWithEditorCamera() {
			Camera editorCamera = SceneView.GetAllSceneCameras()[0];

			foreach (LightingEntity e in Entities()) {
				Collider c = e.GetComponent<Collider>();
				bool includes = c.bounds.Contains(editorCamera.transform.position);
				e.SetActive(includes, includes);
			}
		}

		[ContextMenu("Bake Lights")]
		public void Bake() {
			ActivateAllLights();
			Lightmapping.BakeAsync();
		}
#endif
	}
}
