using System.Collections;
using UnityEngine;
using Assets.Ryooka.Scripts.General;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Ryooka.Scripts.UI {
	public class Turntable : MonoBehaviour {
		public VectorR.Axis axis;
		public float speed;

		float angle {
			get { return transform.localEulerAngles[(int)axis]; }
			set { transform.ModifyLocalEulerAngles(a => a.WithAxis(axis, value)); }
		}

		IEnumerator Start() {
			while (true) {
				var deltaSpeed = speed * Time.deltaTime;
				angle = (angle + deltaSpeed) % 360;
				yield return null;
			}
		}
    }
}
