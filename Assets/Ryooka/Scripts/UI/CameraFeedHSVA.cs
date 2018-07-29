using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.UI {
	// CameraFeed capable of hue/saturation/value/alpha adjustment.
	// You make sure to set the texture's material to HSVRangeShader.
	public class CameraFeedHSVA : CameraFeed {
		[SerializeField]
		Slider adjustHue;

		[SerializeField]
		Slider adjustSaturation;

		[SerializeField]
		Slider adjustValue;

		[SerializeField]
		Slider adjustAlpha;

		Material targetMaterial {
			get { return targetImage.material; }
		}

		protected override void Start() {
			base.Start();

			if (adjustHue) {
				adjustHue.onValueChanged.AddListener(SetHue);
			}

			if (adjustSaturation) {
				adjustSaturation.onValueChanged.AddListener(SetSaturation);
			}

			if (adjustValue) {
				adjustValue.onValueChanged.AddListener(SetValue);
			}

			if (adjustAlpha) {
				adjustAlpha.onValueChanged.AddListener(SetAlpha);
			}
		}

		public void SetHue(float hue) {
			HSVRange.SetHue(targetMaterial, hue);
		}

		public void SetSaturation(float saturation) {
			HSVRange.SetSaturation(targetMaterial, saturation);
		}

		public void SetValue(float value) {
			HSVRange.SetValue(targetMaterial, value);
		}

		public void SetAlpha(float alpha) {
			HSVRange.SetAlpha(targetMaterial, alpha);
		}
	}
}
