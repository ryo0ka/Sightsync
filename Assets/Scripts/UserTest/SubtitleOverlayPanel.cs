using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UserTest {
	public class SubtitleOverlayPanel: Exhibition.DisplayContent {
		public GameObject panel;
		public SportSubtitleText text;
		public GameObject indicator;

		public bool visible;

		// Unity function
		void OnValidate() {
			SetVisible(visible);
		}

		public override void SetVisible(bool visible) {
			this.visible = visible;
			panel.SetActive(visible);
			indicator.SetActive(visible);

			if (visible) {
				text.StartSubtitle();
			}
		}
	}
}
