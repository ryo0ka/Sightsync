using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Extension {
	public class GameObjectExt : MonoBehaviour {
		public void ToggleActive() {
			gameObject.SetActive(!gameObject.activeSelf);
		}
	}
}
