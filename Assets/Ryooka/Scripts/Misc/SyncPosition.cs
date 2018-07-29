using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Misc {
	[ExecuteInEditMode]
	public class SyncPosition : MonoBehaviour {
		public enum UpdateType { UPDATE, LATE_UPDATE, }
		public enum PositionType { WORLD, SCREEN, }

		public Camera targetCamera;
		public Transform target;
		public PositionType positionType;
		public UpdateType updateType;
		public bool executeInEditMode;

		[SerializeField][HideInInspector]
		Vector3 lastPosition;

		void Update() {
			if (updateType != UpdateType.UPDATE) return;
			UpdatePosition();
		}

		void LateUpdate() {
			if (updateType != UpdateType.LATE_UPDATE) return;
			UpdatePosition();
		}

		void UpdatePosition() {
			if (!Application.isPlaying && !executeInEditMode) return;
			switch (positionType) {
				case PositionType.WORLD: UpdateWorldPosition(); break;
				case PositionType.SCREEN: UpdateScreenPosition(); break;
			}
		}

		void UpdateWorldPosition() {
			var p = target.position;
			if (p == lastPosition) return;
			lastPosition = p;
			transform.position = p;
		}

		void UpdateScreenPosition() {
			var p = targetCamera.WorldToScreenPoint(target.position);
			if (p == lastPosition) return;
			lastPosition = p;
			transform.position = targetCamera.ScreenToWorldPoint(p);
        }
	}
}
