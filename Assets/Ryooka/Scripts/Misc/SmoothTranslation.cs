using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.Misc {
	public class SmoothTranslation : MonoBehaviour {
		[Range(0f, 10f)]
		public float amount;

		Vector3 position;

		void LateUpdate() {
			position = transform.position
				.Smooth(position, amount)
				.Map(MathR.ZeroIllegal);
			transform.position = position;
		}
	}
}
