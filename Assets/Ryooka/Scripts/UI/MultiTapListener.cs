using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Ryooka.Scripts.UI {
	public class MultiTapListener: MonoBehaviour, IPointerClickHandler {
		[SerializeField]
		int taps;

		[SerializeField]
		float time;

		[SerializeField]
		UnityEvent onMultipleTap;

		int currentTaps;
		float currentTime;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
			currentTaps += 1;
		}
		
		void Update() {
			bool tapped = currentTaps >= taps; //true if tapped enough times
			bool timed = currentTime >= time; //true if time over
			if (!tapped && !timed) {
				currentTime += Time.deltaTime;
			} else if (tapped && !timed) {
				onMultipleTap.Invoke();
				currentTime = currentTaps = 0;
			} else if (!tapped && timed) {
				currentTime = currentTaps = 0;
			} else if (tapped && timed) {
				currentTime = currentTaps = 0;
			} else {
				throw new ArgumentException(); //shouldn't reach here
			}
		}
	}
}
