using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.UI {
	public class GrowScale : MonoBehaviour {
		public float minSize;
		public float maxSize;
		public float lerpTime;
		public MathR.LerpType lerpType;
		public UnityEvent onStart;
		public UnityEvent onComplete;
		public Transform target;

		bool growing;

		public void SetTarget(Transform target) {
			this.target = target;
		}

		void Reset() {
			minSize = 0;
			maxSize = 10;
			lerpTime = 1;
			lerpType = MathR.LerpType.LINEAR;
		}

		void Scale(float value) {
			transform.localScale = new Vector3(value, value, value);
		}

		IEnumerable<float> IterateScales() {
			return MathR.LerpE(minSize, maxSize, lerpTime, lerpType);
        }

		IEnumerator GrowE() {
			onStart.Invoke();
			if (growing) {
				growing = false;
				yield return null;
			}
			growing = true;
			foreach (var value in IterateScales()) {
				if (!growing) break;
				Scale(value);
				yield return null;
			}
			growing = false;
			onComplete.Invoke();
		}

		IEnumerator GrowFromE(Transform target) {
			var g = GrowE();
			while (g.MoveNext()) {
				transform.position = target.position;
				yield return null;
			}
		}

		public void GrowFrom(Transform target) {
			StartCoroutine(GrowFromE(target));
		}

		public void Grow() {
			StartCoroutine(GrowFromE(target));
		}

		public void Minimize() {
			Scale(minSize);
			growing = false;
		}

		public void Maximize() {
			Scale(maxSize);
			growing = false;
		}
	}
}
