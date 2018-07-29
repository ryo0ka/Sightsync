using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Navigation {
	[RequireComponent(typeof(Collider))]
	[DisallowMultipleComponent]
	public class LightingEntity : LightingManager.Entity {
		[SerializeField]
		LightingManager manager;

		// Active state (enabled/disabled) of pixel lights are managed by this script.
		// - Shown when the player enters the box, and
		// - hidden when the player exits the box.
		[NonSerialized]
		private List<Light> lights;

		// Active state of reflection probes are managed by this script.
		[NonSerialized]
		private List<ReflectionProbe> probes;
		
		void Reset() {
			SetManager(FindObjectOfType<LightingManager>());
		}

		void OnValidate() {
			if (manager != null) {
				SetManager(manager);
			}
		}

		private void Awake() {
			lights = PixelLights().ToList();
			probes = ReflectionProbes().ToList();
		}

		void OnTriggerEnter(Collider collider) {
			if (manager.ConfirmPlayer(collider)) {
				SetActive(true, true);
			}
		}
		
		void OnTriggerExit(Collider collider) {
			if (manager.ConfirmPlayer(collider)) {
				SetActive(false, false);
			}
		}

		public override Collider GetCollider() {
			return GetComponent<Collider>();
		}

		public override IEnumerable<Light> PixelLights() {
			return lights ?? //nonseralized
				GetComponentsInChildren<Light>(true)
				.Where(l => l.type != LightType.Directional);
		}

		public override IEnumerable<ReflectionProbe> ReflectionProbes() {
			return probes ?? // nonserialized
				GetComponentsInChildren<ReflectionProbe>(true)
				.AsEnumerable();
		}

		void SetManager(LightingManager manager) {
			this.manager = manager;
			this.manager.Register(this);
		}
	}
}
