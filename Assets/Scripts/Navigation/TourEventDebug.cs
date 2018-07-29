using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Navigation {
	public class TourEventDebug: TourEvent {
		public float time;

		bool mustHalt;

		public override void InvokeEvent(Action onComplete = null) {
			Debug.Log("Event invoked: " + name);

			time = 0;
			mustHalt = false;

			StartCoroutine(StartMyEvent(onComplete));
		}

		public override void Halt() {
			mustHalt = true;
		}

		IEnumerator StartMyEvent(Action onComplete = null) {
			while (time < 3f && !mustHalt) {
				time += Time.deltaTime;
				
				Debug.Log(name);

				yield return null;
			}

			if (onComplete != null) {
				onComplete();
			}

			Debug.Log("Event finished: " + name);
		}
	}
}
	