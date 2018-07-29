using UnityEngine;

namespace Assets.Ryooka.Scripts.UI {
	[RequireComponent(typeof(RectTransform))]
	public class Compass2d : MonoBehaviour {
		[SerializeField]
		Transform target;

		void LateUpdate() {
			Vector3 angles = transform.eulerAngles;
			angles.z = target.eulerAngles.y;
			transform.eulerAngles = angles;
		}
	}
}
