using Tango;
using UnityEngine;
using UnityEngine.UI;
using Assets.Ryooka.Scripts.Sensor;

namespace Assets.Scripts.Tango {
	// Ensure that a given camera's FOV is synchronized with Tango's camera feed.
	[RequireComponent(typeof(Camera))]
	public class TangoFovCamera : MonoBehaviour, ITangoLifecycle {
		// Update FOV of targetCamera.
		// Optional.
		[SerializeField]
		Button updateFovButton;

		TangoApplication tango;

		Camera targetCamera;

		void Awake() {
			targetCamera = GetComponent<Camera>();
		}

		void Start() {
			tango = FindObjectOfType<TangoApplication>();
			tango.Register(this);

			DeviceOrientationListener.onOrientationChanged += _ => {
				UpdateFOV();
			};

			if (updateFovButton) {
				updateFovButton.onClick.AddListener(() => UpdateFOV());
			}
		}

		void ITangoLifecycle.OnTangoPermissions(bool permissionsGranted) {
		}

		void ITangoLifecycle.OnTangoServiceConnected() {
			UpdateFOV();
		}

		void ITangoLifecycle.OnTangoServiceDisconnected() {
		}
		
		void UpdateFOV() {
			if (!isActiveAndEnabled) return;
			if (!tango.IsServiceConnected) return;

			switch (Screen.orientation) {
				case ScreenOrientation.LandscapeLeft:
				case ScreenOrientation.LandscapeRight:
					SetFov(vertical: true);
					break;
				case ScreenOrientation.Portrait:
				case ScreenOrientation.PortraitUpsideDown:
					SetFov(vertical: false);
					break;
				case ScreenOrientation.AutoRotation:
					switch (Input.deviceOrientation) {
						case DeviceOrientation.LandscapeLeft:
						case DeviceOrientation.LandscapeRight:
							SetFov(vertical: true);
							break;
						case DeviceOrientation.Portrait:
						case DeviceOrientation.PortraitUpsideDown:
							SetFov(vertical: false);
							break;
					}
					break;
			}
		}

		void SetFov(bool vertical) {
			TangoCameraIntrinsics ccIntrinsics = new TangoCameraIntrinsics();
			VideoOverlayProvider.GetIntrinsics(TangoEnums.TangoCameraId.TANGO_CAMERA_COLOR, ccIntrinsics);

			float fov = (vertical)
				? 2f * Mathf.Atan(0.5f * ccIntrinsics.height / (float)ccIntrinsics.fy) * Mathf.Rad2Deg
				: 2f * Mathf.Atan(0.5f * ccIntrinsics.width  / (float)ccIntrinsics.fx) * Mathf.Rad2Deg;
			
			if (float.IsNaN(fov) || float.IsInfinity(fov)) {
				// Tango API itself should have produced a warning message for this case
			} else {
				targetCamera.fieldOfView = fov;
				Debug.Log("FOV is set: " + targetCamera.fieldOfView);
			}
		}
	}
}
