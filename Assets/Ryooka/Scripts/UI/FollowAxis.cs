using UnityEngine;
using Assets.Ryooka.Scripts.General;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Ryooka.Scripts.UI {
	[ExecuteInEditMode]
	public class FollowAxis : MonoBehaviour {
		public Transform target;
		public VectorR.Axis from;
		public VectorR.Axis to;
		public bool flip;
		public float initialAngle;
		public bool executeInEditMode;

		float lastValue;

		void Reset() {
			initialAngle = 0;
			executeInEditMode = false;
		}

		void LateUpdate() {
			if (!Application.isPlaying && !executeInEditMode) return;
			float currentValue = target.localEulerAngles[(int)from];
			if (currentValue == lastValue) return;
			lastValue = currentValue;
			var value = currentValue * ((flip) ? -1 : 1) + initialAngle;
			transform.ModifyLocalEulerAngles(a => a.WithAxis(to, value));
		}
	}
}
