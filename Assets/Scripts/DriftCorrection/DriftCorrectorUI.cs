using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Ryooka.Scripts.General;
using Assets.Ryooka.Scripts.EditorExtension;
using Assets.Ryooka.Scripts.Extension;
using System.IO;

namespace Assets.Scripts.DriftCorrection {
	class DriftCorrectorUI : MonoBehaviour {
		[SerializeField]
		DriftCorrector corrector;

		[SerializeField]
		[UnfoldInInspector]
		Poolable.Pool panels;

		[SerializeField]
		Button constructButton;

		[SerializeField]
		Slider verticalCorrectionSlider;

		[SerializeField]
		Slider correctionRadiusSlider;

		[SerializeField]
		Button resetDriftCorrectionButton;

		[SerializeField]
		InputField fileNameField;

		[SerializeField]
		Button loadButton;

		[SerializeField]
		Button saveButton;

		// Prevents sliders from defining undesired grids
		// when they are centered (initialized) to properly reflect current grid's value.
		// There's no official way to temporarily inactivate callbacks of Slider.
		bool driftSlidersInactive;

		// MonoBehaviour
		void Start() {
			constructButton.onClick.AddListener(ConstructField);
			verticalCorrectionSlider.onValueChanged.AddListener(SetHeightCorrection);
			correctionRadiusSlider.onValueChanged.AddListener(SetCorrectionRadius);
			resetDriftCorrectionButton.onClick.AddListener(ResetDriftCorrection);
			loadButton.onClick.AddListener(LoadFromFile);
			saveButton.onClick.AddListener(SaveFile);

			corrector.onPlayerPositionChanged += ResetDriftSliders;
        }

		// MonoBehaviour
		void Update() {
			float cameraHeight = Camera.main.transform.position.y;
			panels.original.transform.parent.SetPosition(y: cameraHeight + 3f);
		}

		//TODO untested
		void ResetDriftSliders(IntVector2 position) {
			driftSlidersInactive = true;

			var definedC = corrector.GetDefined(position);

			if (definedC.HasValue) {
				// If the grid is defined, get both correction and radius back to sliders.
				verticalCorrectionSlider.value = definedC.Value.height;
				correctionRadiusSlider.value = definedC.Value.radius * corrector.FieldScale;
			} else {
				// Otherwise get correction back to correction slider. Radius is zero.
				verticalCorrectionSlider.value = corrector.Get(position).height;
				correctionRadiusSlider.value = 0;
			}

			driftSlidersInactive = false;
		}

		void SetCorrectionRadius(float radius) {
			SetCorrection(c => {
				c.radius = radius / corrector.FieldScale;
				return c;
			});
		}

		void SetHeightCorrection(float height) {
			SetCorrection(c => {
				c.height = height;
				return c;
			});
		}

		void ResetDriftCorrection() {
			corrector.RemoveAll();
		}
		
		//TODO untested - world/self system
		void SetCorrection(Func<DriftCorrector.Correction, DriftCorrector.Correction> f) {
			if (!driftSlidersInactive) {
				IntVector2 position = corrector.WorldToGridPosition(Camera.main.transform.position);
				corrector.Set(position, f(corrector.Get(position)));
			}
		}

		void ConstructField() {
			corrector.Construct();
			MapPanels();
		}

		float Alpha(float height) {
			float minHeight = verticalCorrectionSlider.minValue;
			float maxHeight = verticalCorrectionSlider.maxValue;
			return MathR.Map(height, minHeight, maxHeight, 0, 1);
		}

		void MapPanels() {
			// Reset all panels
			foreach (var panel in panels) {
				panel.Pooled = true;
				panel.gameObject.SetActive(false);
			}

			// Re-construct panels
			foreach (IntVector2 p in corrector.Grids()) {
				var c = corrector.Get(p);

				var panel = panels.Unpool();
				panel.gameObject.SetActive(true);

				panel.transform.position = corrector.GridToWorldPosition(p);
				panel.transform.SetPosition(y: panels.original.transform.position.y);
				panel.transform.localScale = (corrector.FieldScale).To3();
                panel.GetComponent<Renderer>().SetColor(a: Alpha(c.height));
			}
		}

		void LoadFromFile() {
			corrector.Deserialize(File.ReadAllLines(DataFilePath())[0]);
		}

		void SaveFile() {
			File.WriteAllLines(DataFilePath(), new string[] { corrector.Serialize() });
		}

		string DataFilePath() {
			return IOUtil.PersistentDataFilePath(fileNameField.text, "ssdc");
		}
	}
}
