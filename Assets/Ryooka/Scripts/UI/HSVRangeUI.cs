using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.UI {
	public class HSVRangeUI : MonoBehaviour {
		[SerializeField]
		Graphic targetGraphic;
		
		[SerializeField]
		Slider adjustHue;
		
		[SerializeField]
		Slider adjustSaturation;
		
		[SerializeField]
		Slider adjustValue;

		[SerializeField]
		Slider adjustAlpha;

		Material targetMaterial {
			get { return targetGraphic.material; }
		}

		void Start() {
			if (adjustHue) {
				adjustHue.value = HSVRange.GetHue(targetMaterial);
				adjustHue.onValueChanged.AddListener(hue => {
					HSVRange.SetHue(targetMaterial, hue);
				});
			}

			if (adjustSaturation) {
				adjustSaturation.value = HSVRange.GetSaturation(targetMaterial);
				adjustSaturation.onValueChanged.AddListener(saturation => {
					HSVRange.SetSaturation(targetMaterial, saturation);
				});
			}

			if (adjustValue) {
				adjustValue.value = HSVRange.GetValue(targetMaterial);
				adjustValue.onValueChanged.AddListener(value => {
					HSVRange.SetValue(targetMaterial, value);
				});
			}

			if (adjustAlpha) {
				adjustAlpha.value = HSVRange.GetAlpha(targetMaterial);
                adjustAlpha.onValueChanged.AddListener(alpha => {
					HSVRange.SetAlpha(targetMaterial, alpha);
				});
			}
		}
	}
}
