using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.UI {
	// Simple interface to swich on/off children objects
	// so one of them is active while the rest is inactive.
	// Designed to make up a pager structure in a developer UI.
	// If you want a pager in a frontend UI, you'd better make your own.
	[ExecuteInEditMode]
	public class Pager : MonoBehaviour {

		[SerializeField]
		bool gotoZeroOnStart;
		
		[SerializeField]
		int activeIndex;

		[SerializeField]
		Button succeedButton;

		[SerializeField]
		Button reverseButton;

		// Objects listed here would be ignored when they appear as children.
		[SerializeField]
		List<GameObject> exclude;
		
		// Used in inspector.
		// When script is attached to an object for the first time,
		// manually add an element to this list and set it true in inspector.
		[SerializeField]
		bool[] inspectorUI;

		void Start() {
			if (succeedButton) {
				succeedButton.onClick.AddListener(MoveNext);
			}

			if (reverseButton) {
				reverseButton.onClick.AddListener(MoveBack);
			}

			if (Application.isPlaying && gotoZeroOnStart) {
				SetActivePage(0);
			}
		}
		
		void OnValidate() {
			for (int i = 0; i < inspectorUI.Length; i++) {
				if (inspectorUI[i] && i != activeIndex) {
					SetActivePage(i);
					break;
				}
			}
		}

		void SetActivePage(int index) {
			List <GameObject> children = GetChildren();

			int? _fixedIndex = FixIndex(index, children.Count);

			if (_fixedIndex.HasValue) {
				int fixedIndex = _fixedIndex.Value;

				for (int i = 0; i < children.Count; i++) {
					children[i].SetActive(i == fixedIndex);
				}

				activeIndex = fixedIndex;
			}

			UpdateInspectorUI();
		}

		// TODO super inefficient atm.
		// Don't use this in a frontend UI.
		List<GameObject> GetChildren() {
			List<GameObject> children = new List<GameObject>();

			foreach (Transform _child in transform) {
				GameObject child = _child.gameObject;

				if (!exclude.Contains(child)) {
					children.Add(child);
				}
			}

			return children;
		}

		int? FixIndex(int index, int length) {
			if (length == 0) {
				return null;
			} else if (index < 0) {
				return length - 1;
			} else {
				return index % length;
			}
		}

		void UpdateInspectorUI() {
#if UNITY_EDITOR
			inspectorUI = new bool[transform.childCount];
			inspectorUI[activeIndex] = true;
#endif
		}

		public void MoveNext() {
			SetActivePage(activeIndex + 1);
		}

		public void MoveBack() {
			SetActivePage(activeIndex - 1);
		}

		public int GetTotalPageCount() {
			return GetChildren().Count;
		}

		public int GetCurrentPageNumber() {
			return activeIndex;
		}
	}
}
