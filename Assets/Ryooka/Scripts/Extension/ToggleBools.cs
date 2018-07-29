using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.Extension {
	[RequireComponent(typeof(Toggle))]
	public class ToggleBools : MonoBehaviour {
		public UnityEvent onValueChangedTrue;
		public UnityEvent onValueChangedFalse;

		void Awake() {
			foreach (var toggle in GetComponents<Toggle>()) {
				toggle.onValueChanged.AddListener(b => {
					if (b) onValueChangedTrue.Invoke();
					else onValueChangedFalse.Invoke();
				});
			}
		}
	}
}
