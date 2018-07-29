using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Assets.Ryooka.Scripts.Sensor;

namespace Assets.Ryooka.Scripts.Test {
	public class TestGeolocationData : MonoBehaviour {
		IEnumerator Start() {
			Input.location.Start(10, 10);
			foreach (var _ in Input.location.Try(5, 2)) yield return _;
			if (Input.location.status != LocationServiceStatus.Running) {
				Debug.LogAssertion("Location service unavailable");
			}
		}

		public void Log() {
			Debug.LogFormat("TS: {0}, HA: {1}, VA: {2}, ALT: {3}, LAT: {4}, LONG: {5}",
				Input.location.lastData.timestamp,
				Input.location.lastData.horizontalAccuracy,
				Input.location.lastData.verticalAccuracy,
                Input.location.lastData.altitude,
				Input.location.lastData.latitude,
				Input.location.lastData.longitude);
		}

		public IEnumerable<bool> ObserveServiceUpdate() {
			double lastTimeStamp = 0;
			while (true) {
				double currentTimeStamp = Input.location.lastData.timestamp;
				bool updated = lastTimeStamp != currentTimeStamp;
				if (updated) lastTimeStamp = currentTimeStamp;
				yield return updated;
			}
		}
	}
}
