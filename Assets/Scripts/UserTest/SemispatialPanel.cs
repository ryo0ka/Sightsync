using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UserTest {
	public class SemispatialPanel: Exhibition.DisplayContent {
		public GameObject panel;
		public GameObject indicator;

		public bool visible;

		// Unity function
		void OnValidate() {
			SetVisible(visible);
		}
		
		public override void SetVisible(bool visible) {
			this.visible = visible;
			if (panel && indicator) {
				panel.SetActive(visible);
				indicator.SetActive(visible);
			}
		}
	}
}
