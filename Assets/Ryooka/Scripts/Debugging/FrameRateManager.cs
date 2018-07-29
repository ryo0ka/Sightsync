using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.Debugging {
    public class FrameRateManager : MonoBehaviour {
		[Range(1, 120)]
		public int targetFrameRate;

		public Text outputText;

		public Slider slider;

		float deltaTime;

		void Reset() {
			outputText = GetComponent<Text>();

			slider = GetComponent<Slider>();
		}

		void Start() {
			if (slider) {
				slider.onValueChanged.AddListener(v => {
					targetFrameRate = Application.targetFrameRate = (int)v;
				});
			}
		}

		void Update() {
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			float currentFrameRate = 1.0f / deltaTime;

			if (outputText) {
				outputText.text = currentFrameRate.ToString();
			}
		}

		void OnValidate() {
			Application.targetFrameRate = targetFrameRate;
		}
    }
}
