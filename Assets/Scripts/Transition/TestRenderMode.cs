using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Transition {
	[ExecuteInEditMode]
	public class TestRenderMode : MonoBehaviour {
		public float mode;

		private void Update() {
			Renderer r = GetComponent<Renderer>();
			if (r != null) {
				mode = r.sharedMaterial.GetFloat("_Mode");
			}
		}
	}
}
