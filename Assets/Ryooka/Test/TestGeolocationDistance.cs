using System;
using UnityEngine;
using Assets.Ryooka.Scripts.Sensor;

namespace Assets.Ryooka.Scripts.Test {
	public class TestGeolocationDistance : MonoBehaviour {
		string c1a = "34.208641";
		string c1o = " -118.340537";
		string c2a = "34.205882";
		string c2o = "-118.340409";

		void OnGUI() {
			c1a = GUILayout.TextField(c1a);
			c1o = GUILayout.TextField(c1o);
			c2a = GUILayout.TextField(c2a);
			c2o = GUILayout.TextField(c2o);

			try {
				var c1 = new Geolocation(
					latitude: float.Parse(c1a),
					longitude: float.Parse(c1o));
				var c2 = new Geolocation(
					latitude: float.Parse(c2a),
					longitude: float.Parse(c2o));
				GUILayout.Label(c1.Distance(c2).ToString());
			} catch (FormatException) {
				GUILayout.Label("FormatException");
			}
		}
	}
}
