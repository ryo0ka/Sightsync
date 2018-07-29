using UnityEngine;
using UnityEngine.UI;
using Assets.Ryooka.Scripts.Extension;
using Assets.Ryooka.Scripts.EditorExtension;
using Assets.Ryooka.Scripts.UI;
using System.IO;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.Misc {
	public class TransformManager: MonoBehaviour {
		[SerializeField]
		bool highlightSelectedObject;

		[SerializeField]
		string fileName;

		[SerializeField]
		Button saveButton;

		[SerializeField]
		Button loadButton;

		[SerializeField]
		RectTransform selectorBox;

		[SerializeField]
		LabeledButton selectButtonPrototype;

		[SerializeField]
		[UnfoldInInspector]
		UI.TransformController adjuster;

		[SerializeField]
		[UnfoldInInspector]
		TransformSerializer manager; //TODO: rename to serializer
		
		void Start() {
			InitiateAllSelectors();
			UnhighlightAll();

			if (saveButton) saveButton.onClick.AddListener(Save);
			if (loadButton) loadButton.onClick.AddListener(Load);
		}

		void InitiateAllSelectors() {
			foreach (var t in manager.AllTargets())
				InitiateSelectorOf(t);
		}

		void InitiateSelectorOf(Transform target) {
			LabeledButton controller = Instantiate(selectButtonPrototype);
			controller.transform.parent = selectorBox;
			controller.onClick.AddListener(() => Select(target));
			controller.text = target.name;
			controller.gameObject.SetActive(true);

			// Vectical Layer Group has a scalling bug
			// that can be worked around by force-resetting the scale.
			controller.transform.localScale = Vector3.one;
		}

		void Select(Transform target) {
			adjuster.target = target;
			HighlightTarget(target.gameObject);
		}

		void HighlightTarget(GameObject target) {
			UnhighlightAll();
			ToggleHighlight(target, true);
		}

		void UnhighlightAll() {
			foreach (var t in manager.AllTargets())
				ToggleHighlight(t.gameObject, false);
		}

		//applies the effect to all the children's renderer too.
		void ToggleHighlight(GameObject target, bool selected) {
			if (!highlightSelectedObject) return;

			foreach (var r in target.GetComponents<Renderer>(-1, true))
				r.SetColor(r: selected ? .2f : 1f);
		}

		public void Save() {
			File.WriteAllLines(DataFilePath(), manager.Serialize());
		}

		public void Load() {
			manager.Deserialize(File.ReadAllLines(DataFilePath()));
		}

		string DataFilePath() {
			return IOUtil.PersistentDataFilePath(fileName, "txt");
		}
	}
}
