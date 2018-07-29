using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Test {
	public class TestDeltaTime : MonoBehaviour {
		public int frameRate;
		public Text secondsText;

		int frame;
		float tl;

		void OnValidate() {
			Application.targetFrameRate = frameRate;
		}

		void Update() {


			float t = frame++ * Time.deltaTime;

			t = tl = Mathf.Max(t, tl);

			secondsText.text = t.ToString();
		}
	}
}
