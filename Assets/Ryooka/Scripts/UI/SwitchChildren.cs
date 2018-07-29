using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Ryooka.Scripts.UI {
	[ExecuteInEditMode]
	class SwitchChildren : MonoBehaviour {
		[SerializeField]
		float secondsPerFrame;
		
		[SerializeField]
		int currentCount;

		IEnumerator Start() {
			while (true) {
				UpdateFrame();
				yield return new WaitForSeconds(secondsPerFrame);
			}
		}

		void UpdateFrame() {
			var children = new List<GameObject>(Children());

			for (int i = 0; i < children.Count; ++i) {
				var shouldBeOn = i == currentCount;
				children[i].SetActive(shouldBeOn);
			}

			currentCount = (currentCount + 1) % children.Count;

			if (currentCount >= int.MaxValue) {
				currentCount = 0;
			}
		}

		IEnumerable<GameObject> Children() {
			foreach (Transform c in transform) {
				yield return c.gameObject;
			}
		}
	}
}
