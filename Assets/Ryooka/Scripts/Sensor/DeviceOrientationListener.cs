using System;
using System.Collections;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Sensor {
	// https://forum.unity3d.com/threads/device-screen-rotation-event.118638/
	public class DeviceOrientationListener: MonoBehaviour {
		public static event Action<DeviceOrientation> onOrientationChanged;
		public static float checkIntervalSeconds = 0.5f;
		static DeviceOrientation lastOrientation;

		IEnumerator Start() {
			onOrientationChanged += _ => { };
			lastOrientation = Input.deviceOrientation;

			// Check for an Orientation Change
			switch (Input.deviceOrientation) {
				case DeviceOrientation.LandscapeLeft:
				case DeviceOrientation.LandscapeRight:
				case DeviceOrientation.Portrait:
				case DeviceOrientation.PortraitUpsideDown:
					if (lastOrientation != Input.deviceOrientation) {
						lastOrientation = Input.deviceOrientation;
						onOrientationChanged(lastOrientation);
					}
					break;
				default:
					break;
			}

			yield return new WaitForSeconds(checkIntervalSeconds);
		}
	}
}
