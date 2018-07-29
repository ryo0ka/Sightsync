using UnityEngine;

namespace Assets.Ryooka.Test {
	// Consequences:
	//  A': Input field (in app) starts writing.
	//  A": Input field (in app) completes writing.
	//  B': Circle/square button is pressed (paused  while phone is on).
	//  B": App re-gains focus from B'      (resumed while phone is on).
	//  C': Phone is turned off (with app active).
	//  C": Phone is turned on  (with app active).
	//  D": App starts.
	public class TestAndroidResume: MonoBehaviour {
		int count;

		void Start() {
			// Doesn't work (count increments but log won't show up).
			// Invokes when: B', C'.
			AndroidHelper.RegisterPauseEvent(() => {
				Debug.Log("AndroidHelper.PauseEvent: " + count++);
			});

			// Doesn't work (count increments but log won't show up).
			// Invokes when: B", C".
			AndroidHelper.RegisterResumeEvent(() => {
				Debug.Log("AndroidHelper.ResumeEvent: " + count++);
			});
		}

		// Works.
		// True  when: B', C'.
		// False when: B", C", D".
		void OnApplicationPause(bool paused) {
			Debug.Log("MonoBehavior.OnApplicationPause(" + paused + "): " + count++);
		}

		// Works.
		// True  when: A", B", D".
		// False when: A', B'.
		void OnApplicationFocus(bool focus) {
			Debug.Log("MonoBehavior.OnApplicationFocus(" + focus + "): " + count++);
		}
	}
}
