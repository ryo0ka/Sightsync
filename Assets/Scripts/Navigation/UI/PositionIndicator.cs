using UnityEngine;
using Assets.Ryooka.Scripts.General;

namespace Assets.Scripts.Navigation.UI {
	[ExecuteInEditMode]
	public class PositionIndicator : MonoBehaviour {
		[SerializeField]
		Transform target;
		
		[SerializeField]
		Transform spatialIndicator;

		[SerializeField]
		RectTransform overlayIndicator;

		[SerializeField]
		float overlayIndicatorInitialAngle;

		bool shouldHide;

		// Unity function
		void LateUpdate() {
			if (target && spatialIndicator) {
				spatialIndicator.position = target.position;
			}

			if (!shouldHide && target && overlayIndicator) {
				var point = Camera.main.WorldToViewportPoint(target.position);
				overlayIndicator.gameObject.SetActive(true);
				var z = overlayIndicator.localEulerAngles.z;
				if (point.z > 0 && Mathf.Abs(point.x) < 1) {
					overlayIndicator.gameObject.SetActive(false);
				} else if (point.z > 0) {
					if (point.x > 0 && point.x < 10) {
						z = MathR.Map(point.x, 3f, 10f, -45f, -90f);
					} else if (point.x < 0 && point.x > -10) {
						z = MathR.Map(point.x, -3f, -10f, 45f, 90f);
					}
				} else if (point.z < 0 && point.x > -10 && point.x < 10) {
					z = MathR.Map(point.x, -10f, 10f, 90f, -90f) + 180;
				}
				var eu = new Vector3 (0, 0, z);
				overlayIndicator.localEulerAngles = eu;
			}
		}

		public void ActivateWithTarget(Transform target) {
			Debug.LogFormat("PositionIndicator::ActivateWithTarget({0})", target);
			this.target = target;

			spatialIndicator.gameObject.SetActive(true);
			overlayIndicator.gameObject.SetActive(true);

			shouldHide = false;
		}
		
		public void Hide() {
			Debug.LogFormat("PositionIndicator::Hide");

			spatialIndicator.gameObject.SetActive(false);
			overlayIndicator.gameObject.SetActive(false);

			shouldHide = true;
		}
	}
}
