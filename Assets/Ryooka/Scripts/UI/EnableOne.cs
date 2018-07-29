using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Ryooka.Scripts.UI {
	[ExecuteInEditMode]
	public class EnableOne : MonoBehaviour {
		public bool active;
		public int enabledIndex;
		public bool disableAll;
		public List<GameObject> objects;

		void OnValidate() {
			if (!active) return;
			Enable(enabledIndex);
			if (disableAll) {
				DisableAll();
				disableAll = false;
			}
		}

		public void SetEnabledIndex(int index) {
			enabledIndex = index;
		}

		public void Enable(int index) {
			if (!active) return;
			enabledIndex = index;
			for (int i = 0; i < objects.Count; ++i) {
				var o = objects.ElementAt(i);
				if (o != null) o.SetActive(i == index);
			}
		}

		public void Enable() {
			Enable (enabledIndex);
		}

		public void DisableAll() {
			if (!active) return;
			Enable(-1);
		}

		public GameObject Enabled() {
			return objects.ElementAt(enabledIndex);
		}
	}
}
