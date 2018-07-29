using UnityEngine;
using UnityEngine.Events;

namespace Assets.Ryooka.Scripts.UI {
    public class KeyInputListener : MonoBehaviour {
		public KeyCode key;
		public UnityEvent onPressed;

		bool pressed;

        void Update() {
			if (Input.GetKeyDown(key)) {
				if (!pressed) {
					onPressed.Invoke();
				}
				pressed = true;
			} else if (Input.GetKeyUp(key)) {
				pressed = false;
			}
		}
    }
}
