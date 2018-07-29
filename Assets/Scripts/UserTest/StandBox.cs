using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.UserTest {
	public class StandBox : MonoBehaviour {
		[SerializeField]
		bool visible;

		[HideInInspector]
		[SerializeField]
		Collider player;

		[HideInInspector]
		[SerializeField]
		UnityEvent onEnter;
		
		[HideInInspector]
		[SerializeField]
		UnityEvent onExit;
		
		// Unity function
		void OnTriggerEnter(Collider other) {
			if (other == player) {
				onEnter.Invoke();
			}
		}

		// Unity function
		void OnTriggerExit(Collider other) {
			if (other == player) {
				onExit.Invoke();
			}
		}
		
		public void ClearEvents() {
			onEnter.RemoveAllListeners();
			onExit.RemoveAllListeners();
		}
	
		public void OnEnter(UnityAction action) {
			onEnter.AddListener(action);
		}

		public void OnExit(UnityAction action) {
			onExit.AddListener(action);
		}

		public void SetVisible(bool visible) {
			this.visible = visible;
			GetComponent<Renderer>().enabled = visible;
			foreach (var child in GetComponentsInChildren<Renderer>()) {
				child.enabled = visible;
			}
		}

		public void ToggleVisible() {
			visible = !visible;
			SetVisible(visible);
		}

		public void SetPlayer(Collider collider) {
			player = collider;
		}
	}
}
