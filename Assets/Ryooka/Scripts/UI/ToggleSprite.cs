using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.UI {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Image))]
	public class ToggleSprite : MonoBehaviour {
		public bool isOn;

		public Sprite on;
		public Sprite off;

		public void SwapSprite(bool value) {
			isOn = value;
			GetComponent<Image>().sprite = (value) ? on : off;
		}

		void OnValidate() {
			SwapSprite(isOn);
		}
	}
}
