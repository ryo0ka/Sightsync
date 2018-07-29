#if false

using Vuforia;
using UnityEngine;

namespace Assets.Ryooka.Scripts.External {
	public class VuforiaInitialization : MonoBehaviour {
		void Start() {
			VuforiaBehaviour.Instance.RegisterVuforiaStartedCallback(SetFocusMode);
			VuforiaBehaviour.Instance.RegisterOnPauseCallback((b) => { if (!b) SetFocusMode(); });
		}
		
		void SetFocusMode() {
			CameraDevice.Instance.SetFocusMode(
				CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
		}
	}
}

#endif