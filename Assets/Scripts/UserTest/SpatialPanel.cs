using System;
using UnityEngine;

namespace Assets.Scripts.UserTest {
	public class SpatialPanel: Exhibition.DisplayContent {
		public GameObject panel;
		public bool visible;

		// Unity function
		void OnValidate() {
			SetVisible(visible);
		}

		public override void SetVisible(bool visible) {
			this.visible = visible;
			panel.SetActive(visible);
		}
	}
}
