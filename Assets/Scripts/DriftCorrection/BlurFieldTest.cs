using Assets.Ryooka.Scripts.EditorExtension;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DriftCorrection {
	public class BlurFieldTest : MonoBehaviour {
		class FloatValue {
			public float v;
			public float r;
		}

		class FloatType: BlurField<FloatValue>.ValueType {
			public static FloatType instance = new FloatType();

			FloatValue BlurField<FloatValue>.ValueType.Blend(FloatValue v1, FloatValue v2) {
				return new FloatValue { v = (v1.v + v2.v) / 2 };
			}

			FloatValue BlurField<FloatValue>.ValueType.Identity() {
				return new FloatValue();
			}

			FloatValue BlurField<FloatValue>.ValueType.Multiply(FloatValue v1, float time) {
				return new FloatValue { v = v1.v * time };
			}

			float BlurField<FloatValue>.ValueType.Radius(FloatValue v) {
				return v.r;
			}

			FloatValue BlurField<FloatValue>.ValueType.Sum(FloatValue v1, FloatValue v2) {
				return new FloatValue { v = v1.v + v2.v };
			}
		}

		BlurField<FloatValue> map;

		[SerializeField]
		Button blurButton;

		[SerializeField]
		IntVector2 scale;

		[SerializeField]
		[UnfoldInInspector]
		Poolable.Pool panelPool;

		void Start() {
			blurButton.onClick.AddListener(ApplyBlur);

			map = new BlurField<FloatValue>(FloatType.instance, scale);
			map.DefineGrid(10, 10, new FloatValue { v = -0.5f, r = 10 });
			map.DefineGrid(20, 20, new FloatValue { v = 0.3f, r = 8 });
			map.DefineGrid(15, 15, new FloatValue { v = 0.5f, r = 8 });
			map.MapOutPredefinedGrids();
			InitializePanels();

		}

		void ApplyBlur() {
			map.Blur();
			InitializePanels();
		}

		void InitializePanels() {
			foreach (var panel in panelPool) {
				panel.Pooled = true;
				panel.gameObject.SetActive(false);
			}

			foreach (var coord in map.Grids()) {
				Vector3 position = new Vector3(coord.x, 0, coord.y);
				FloatValue value = map.GetValue(coord);

				float v = (value == null) ? 0.5f : value.v + 0.5f;
				Color color = new Color(v, 0, 0);

				string text = v.ToString();
				if (text.Length > 4) text = text.Substring(0, 4);

				Poolable panel = panelPool.Unpool();
				panel.gameObject.SetActive(true);

				panel.transform.position = position;
				panel.GetComponent<Renderer>().material.color = color;
				panel.GetComponentInChildren<TextMesh>().text = text;
			}
		}
	}
}
