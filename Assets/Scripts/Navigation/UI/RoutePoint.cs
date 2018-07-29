using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Navigation.UI {
	public class RoutePoint : MonoBehaviour {
		// Invoke an ending animation and destroy itself.
		public void Finish() {
			Destroy(gameObject);
		}
	}
}
