using UnityEngine;
using Assets.Ryooka.Scripts.Extension;
using Assets.Ryooka.Scripts.EditorExtension;

namespace Assets.Scripts.Transition {
	public class UpDownAnimation : MonoBehaviour {
		[SerializeField]
		[UnfoldInInspector]
		TwoWayTransition pp;

		public float maxDepth;

		float upPosition;

		// True if this object should be rising up or at its top position.
		public bool ShouldUp { get; set; }

		private void Reset() {
			upPosition = 10f;
		}

		private void Start() {
			upPosition = transform.localPosition.y;
		}

		private void Update() {
			float position;
			bool done;
			pp.Update(ShouldUp, out position, out done);

			float depth = Mathf.Abs(maxDepth) * position;
			transform.SetPosition(y: upPosition - depth);
		}
	}
}
