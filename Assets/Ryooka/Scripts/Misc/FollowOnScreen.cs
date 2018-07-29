using UnityEngine;

namespace Assets.Ryooka.Scripts.Misc {
	public class FollowOnScreen: MonoBehaviour {
		public Camera targetCamera;
		public Transform targetObject;

		void Reset() {
			targetCamera = Camera.main;
		}

		void Update() {
			if (targetObject == null) return;
			var posOnScreen = targetCamera.WorldToScreenPoint(targetObject.position);
			transform.position = posOnScreen;
		}

		public void SetTargetObject(Transform target) {
			targetObject = target;
		}

		public void SetTargetObject(GameObject target) {
			SetTargetObject(target.transform);
		}
	}
}
