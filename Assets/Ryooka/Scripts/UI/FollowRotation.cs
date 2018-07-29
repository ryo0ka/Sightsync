using System;
using UnityEngine;

namespace Assets.Ryooka.Scripts.UI {
	[ExecuteInEditMode]
	public class FollowRotation : MonoBehaviour {
		public enum UpdateType { UPDATE, LATE_UPDATE, }

		public Transform target;
		public Vector3 scale;
		public Vector3 initial;
		public Space targetSpace;
		public Space thisSpace;
		public UpdateType updateType;
		public bool executeInEditMode;

		Vector3 lastRotation;

		void Reset() {
			scale = Vector3.one;
			initial = Vector3.zero;
			targetSpace = thisSpace = Space.Self;
			updateType = UpdateType.LATE_UPDATE;
			executeInEditMode = false;
		}

		void OnValidate() {
			scale.x = (scale.x == 0) ? 0 : 1;
			scale.y = (scale.y == 0) ? 0 : 1;
			scale.z = (scale.z == 0) ? 0 : 1;
		}

		void Update() {
			if (!Active()) return;
			if (updateType != UpdateType.UPDATE) return;
			Follow();
		}

		void LateUpdate() {
			if (!Active()) return;
			if (updateType != UpdateType.LATE_UPDATE) return;
			Follow();
		}

		Vector3 GetTargetRotation() {
			switch (targetSpace) {
				case Space.Self: return target.localEulerAngles;
				case Space.World: return target.eulerAngles;
				default: throw new ArgumentException();
			}
		}

		void SetRotation(Vector3 rotation) {
			switch (thisSpace) {
				case Space.Self: transform.localEulerAngles = rotation; break;
				case Space.World: transform.eulerAngles = rotation; break;
				default: throw new ArgumentException();
			}
		}

		void Follow() {
			var currentRotation = GetTargetRotation();
			if (currentRotation == lastRotation) return;
			lastRotation = currentRotation;
			var scaledRotation = new Vector3(
				x: currentRotation.x * scale.x - initial.x,
				y: currentRotation.y * scale.y - initial.y,
				z: currentRotation.z * scale.z - initial.z);
			SetRotation(scaledRotation);
		}

		bool Active() {
			return Application.isPlaying || executeInEditMode;
		}
	}
}
