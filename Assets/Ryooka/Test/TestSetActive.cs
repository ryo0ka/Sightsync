using UnityEngine;

namespace Assets.Ryooka.Test {
	public class TestSetActive : MonoBehaviour {
		// Works even if gameObject is inactive.
		public void SetActive(bool active) {
			gameObject.SetActive(active);
		}
	}
}
