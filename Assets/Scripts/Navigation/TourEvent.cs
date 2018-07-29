using System;
using UnityEngine;

namespace Assets.Scripts.Navigation {
	public abstract class TourEvent : MonoBehaviour {
		// Invoke the event. When the event is ended, invoke onComplete.
		public abstract void InvokeEvent(Action onComplete = null);

		// Force quit the invoked event right now.
		public abstract void Halt();
	}
}
