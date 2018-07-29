using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Ryooka.Scripts.UI {
	[ExecuteInEditMode]
	public class RemainRotation: MonoBehaviour {
		public Vector3 desiredRotation;
		public bool executeInEditMode;

		void LateUpdate() {
			if (!Application.isPlaying && !executeInEditMode) return;
			transform.eulerAngles = desiredRotation;
		}
	}
}
