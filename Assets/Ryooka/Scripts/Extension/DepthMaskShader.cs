using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.Extension {
	public class DepthMaskShader : MonoBehaviour {
		public Material depthMaskMaterial;
		public string propertyName = "_ColorMask";

		public Toggle activateToggle;

		private void OnValidate() {
			if (depthMaskMaterial) {
				if (!depthMaskMaterial.HasProperty(propertyName)) {
					Debug.LogAssertion("Not supported material: " + depthMaskMaterial);
					depthMaskMaterial = null;
				}
			}
		}

		private void Start() {
			if (activateToggle) {
				activateToggle.onValueChanged.AddListener(SetActiveDepthMask);
			}
		}

		public void SetActiveDepthMask(bool active) {
			depthMaskMaterial.SetFloat(propertyName, (active) ? 0f : 15f);

			if (activateToggle) {
				activateToggle.isOn = active;
			}
		}
	}
}
