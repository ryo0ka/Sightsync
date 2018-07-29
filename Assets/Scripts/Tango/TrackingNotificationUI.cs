using Assets.Ryooka.Scripts.External;
using System.Collections;
using Tango;
using UnityEngine;

namespace Assets.Scripts.Tango {
	// Manages a UI that notifies the state of Tango service's motion tracking.
	// `trackingIndicator` will be presented:
	//   - from when the motion tracking is established
	//   - to when a long enough time has passed since the moment
	//     (so that the indicator won't persist in the screen).
	// `lostIndicator`, on other hands, will be presented
	//   - while the motion tracking is lost.
	public class TrackingNotificationUI: MonoBehaviour, ITangoPose, ITangoLifecycle {
		// Notify that tracking is active.
		[SerializeField]
		GameObject trackingIndicator;

		// Notify that tracking is lost.
		[SerializeField]
		GameObject lostIndicator;

		[SerializeField]
		GameObject indicatorParent;

		// Seconds waited before concluding that the motion tracking may be lost.
		// Start counting when isTracking is false and continue counting while false.
		[SerializeField]
		float lostThresholdSeconds;

		// Seconds waited to hide indicators when they don't have to be presented.
		// Start counting when isTracking is true, and continue counting while true.
		[SerializeField]
		float hideIndicatorsSeconds;

		// Determine if the motion tracking is active or lost.
		bool isTracking;

		// Determine if the Tango service is running.
		// If false, this UI should present nothing.
		bool isConnected;

		TangoApplication tango;

		IEnumerator Start() {
			// Register this event system to the Tango service.
			tango = FindObjectOfType<TangoApplication>();
			tango.Register(this);

			// Seconds past since the motion tracking has been lost.
			// When this reaches lostThreasholdSeconds, lostIndicator should be presented.
			float lostSeconds = 0;

			// Seconds past since the motion tracking has been active.
			// When this reaches hideIndicatorsSeconds, indicators should be hidden.
			float trackingSeconds = 0;

			while (true) {
				if (isConnected) {
					if (isTracking) {
						trackingSeconds += Time.deltaTime;
						lostSeconds = 0;
					} else {
						lostSeconds += Time.deltaTime;
						trackingSeconds = 0;
					}

					if (isTracking && trackingSeconds < hideIndicatorsSeconds) {
						// trackingIndicator should be presented when
						// the motion tracking is active and not too long time
						// has passed since the tracking was established.
						SetUIState(true);
					} else if (lostSeconds > lostThresholdSeconds) {
						// lostIndicator should be presented when
						// the motion tracking is lost and a long enough time
						// has passed since the tracking was lost.
						SetUIState(false);
					} else {
						// Both indicators should be hidden when
						// the motion tracking has been active for a long enough time,
						// or not a long enough time has passed since the tracking was lost.
						SetUIState(null);
					}
				} else {
					lostSeconds = trackingSeconds = 0;

					// If the Tango service is not connected,
					// the UI shouldn't present anything (should pretend being inactive).
					SetUIState(null);
				}

				yield return null;
			}
		}

		void ITangoPose.OnTangoPoseAvailable(TangoPoseData poseData) {
			if (tango.IsServiceConnected) {
				if (poseData.IsLocalized() || poseData.IsTracking()) {
					isTracking = true;
				} else {
					isTracking = false;

					if (poseData.IsInvalid()) {
						if (tango.m_motionTrackingAutoReset) {
							Debug.Log("TangoApplication is recovering motion tracking...");
						} else {
							Debug.LogWarning("Manually restart Tango service.");
						}
					}	
				}
			}
		}

		void ITangoLifecycle.OnTangoServiceConnected() {
			isConnected = true;
		}

		void ITangoLifecycle.OnTangoServiceDisconnected() {
			isConnected = false;
		}

		void ITangoLifecycle.OnTangoPermissions(bool permissionsGranted) {
			//nothing
		}

		// Apply the given internal state to the UI.
		// There are essentially three states that the UI possibly takes.
		// - If uiState is true , trackingIndicator should be shown, lostIndicator hidden.
		// - If uiState is false, the other way around.
		// - If uiState is none , both indicators should be hidden.
		void SetUIState(bool? uiState) {
			if (uiState.HasValue) {
				indicatorParent.SetActive(true);

				// If tracking is true,  show trackingIndicator and hide lostIndicator.
				// If tracking is false, show lostIndicator and hide trackingIndicator.
				trackingIndicator.SetActive(uiState.Value);
				lostIndicator.SetActive(!uiState.Value);
			} else {
				// If tracking is null, hide both indicators and the parent.
				indicatorParent.SetActive(false);
				trackingIndicator.SetActive(false);
				lostIndicator.SetActive(false);
			}
		}
	}
}
