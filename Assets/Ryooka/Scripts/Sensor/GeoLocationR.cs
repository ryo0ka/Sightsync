using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Sensor {
	public static class GeolocationR {
		public static IEnumerable Try(this LocationService self, int waitSeconds, int waitAttempts) {
			int currentwait = waitAttempts;
			while (Input.location.status != LocationServiceStatus.Running && currentwait > 0) {
				yield return new WaitForSeconds(waitSeconds);
				currentwait--;
			}
		}
	}
}
