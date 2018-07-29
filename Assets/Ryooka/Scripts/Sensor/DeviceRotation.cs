using UnityEngine;

namespace Assets.Ryooka.Scripts.Sensor {
	//http://vr-cto.hateblo.jp/entry/2016/05/02/070000
	//https://forum.unity3d.com/threads/match-unity-camera-with-iphone-camera.128493/
	public class DeviceRotation: MonoBehaviour {
		public bool disableOnEditor;

		double lastCompassUpdateTime = 0;
		Quaternion correction = Quaternion.identity;
		Quaternion targetCorrection = Quaternion.identity;

		void Start() {
			Input.gyro.enabled = true;
			Input.compass.enabled = true;
		}

		void Update() {
			if (Application.isEditor && disableOnEditor) return;
			ObserveCompass();
		}

		void ObserveCompass() {
			Quaternion gorientation = ChangeAxis(Input.gyro.attitude);

			if (Input.compass.timestamp > lastCompassUpdateTime) {
				lastCompassUpdateTime = Input.compass.timestamp;

				Vector3 gravity = Input.gyro.gravity.normalized;
				Vector3 rawvector = CompassRawVector;
				Vector3 flatnorth = rawvector -
					Vector3.Dot(gravity, rawvector) * gravity;

				Quaternion corientation = ChangeAxis(
					Quaternion.Inverse(
						Quaternion.LookRotation(flatnorth, -gravity)));

				// +zを北にするためQuaternion.Euler(0,0,180)を入れる。
				Quaternion tcorrection = corientation *
					Quaternion.Inverse(gorientation) *
					Quaternion.Euler(0, 0, 180);

				// 計算結果が異常値になったらエラー
				// そうでない場合のみtargetCorrectionを更新する。
				if (!IsNaN(tcorrection)) {
					targetCorrection = tcorrection;
				}
			}

			if (Quaternion.Angle(correction, targetCorrection) < 45) {
				correction = Quaternion.Slerp(correction, targetCorrection, 0.02f);
			} else {
				correction = targetCorrection;
			}

			Quaternion rotation = Quaternion.Euler(90, 0, 0) * gorientation;
			transform.localRotation = rotation;
		}
		
		// Androidの場合はScreen.orientationに応じてrawVectorの軸を変換
		Vector3 CompassRawVector {
			get {
				Vector3 ret = Input.compass.rawVector;

				if (Application.platform == RuntimePlatform.Android) {
					switch (Screen.orientation) {
						case ScreenOrientation.LandscapeLeft:
							ret = new Vector3(-ret.y, ret.x, ret.z);
							break;
						case ScreenOrientation.LandscapeRight:
							ret = new Vector3(ret.y, -ret.x, ret.z);
							break;
						case ScreenOrientation.PortraitUpsideDown:
							ret = new Vector3(-ret.x, -ret.y, ret.z);
							break;
					}
				}

				return ret;
			}
		}

		// Quaternionの各要素がNaNもしくはInfinityかどうかチェック
		bool IsNaN(Quaternion q) {
			bool ret =
			float.IsNaN(q.x) || float.IsNaN(q.y) ||
			float.IsNaN(q.z) || float.IsNaN(q.w) ||
			float.IsInfinity(q.x) || float.IsInfinity(q.y) ||
			float.IsInfinity(q.z) || float.IsInfinity(q.w);

			return ret;
		}

		Quaternion ChangeAxis(Quaternion q) {
			return new Quaternion(-q.x, -q.y, q.z, q.w);
		}
	}
}
