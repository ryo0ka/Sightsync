using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Tango;
using Assets.Ryooka.Scripts.External;
using Assets.Scripts.Tango.AdfList;

namespace Assets.Scripts.Tango {
	// Must deal with:
	// - Managing Tango service's life cycle.
	// - Bridging Tango-specific front-end UI (system and UX).
	public class TangoEventManager: MonoBehaviour {
		// UI to notify users that app is trying to connect Tango service.
		public abstract class StartupUI: MonoBehaviour {
			// Invoked when service is prompted to start up.
			// This UI is displayed while the main thread is busy.
			// So you may not assign an animation to the UI.
			public abstract void OnIntended();

			// Invoked when service actually starts up.
			public abstract void OnStarted();
		}

		public bool ensurePermissions;

		public bool reconnectOnSave;

		public StartupUI startupNotification;

		public TrackingNotificationUI trackingUI;

		public TangoAdfListUI selectAdfUI;

		public Button requestPermissionsButton;

		public Text adfNameText;

		// If this is true, the service will learn the area. 
		// Otherwise the service will load a named ADF.
		// If an input name exists in the disk, extend the file; otherwise creates a new file.
		public Toggle learningAreaToggle;

		// Note: don't press this button until the Tango service is up.
		// Saving ADF without the service will crash the whole program.
		public Button saveAdfButton;

		public Button connectServiceButton;
		
		public Button disconnectServiceButton;

		TangoApplication tango;

		AreaDescription selectedADF;

		// Unity function
		void Start() {
			tango = FindObjectOfType<TangoApplication>();

			tango.Register(this);

			if (saveAdfButton) {
				saveAdfButton.onClick.AddListener(() => {
					SaveCurrentADF();
				});
			}

			if (connectServiceButton) {
				connectServiceButton.onClick.AddListener(() => {
					ConnectService();
				});
			}

			if (disconnectServiceButton) {
				disconnectServiceButton.onClick.AddListener(() => {
					DisconnectService();
				});
			}

			if (learningAreaToggle) {
				learningAreaToggle.isOn = tango.m_enableAreaDescriptions;
				learningAreaToggle.onValueChanged.AddListener(shouldLearn => {
					// Note that this won't change learning mode of (currently) running service.
					// This will be applied to service from the next connection.
					tango.m_areaDescriptionLearningMode = shouldLearn;
				});
			}

			if (requestPermissionsButton) {
				requestPermissionsButton.onClick.AddListener(() => {
					Debug.Log("Requesting permissions...");
					tango.RequestPermissions();
					Debug.Log("Requested permissions.");
				});
			}

			if (selectAdfUI) {
				selectAdfUI.onAdfSelected += (adf) => {
					selectedADF = adf;

					// selectedADF may be null if creating a new ADF.
					if (adfNameText) {
						if (selectedADF == null) {
							// If no ADF is currently selected, blank out the text field.
							adfNameText.text = "";
                        } else {
							// If an ADF is selected, reflect the name to the text field.
							adfNameText.text = selectedADF.GetMetadata().m_name;
						}
					}
				};
			}
		}

		// This is invoked when:
		// - User presses the device's button to unfocus from the app.
		// - User re-opens the app.
		// - User turns of (let sleep) the device.
		// - User turns on the device with the app up-front.
		// - The app launches.
		// - The app closes.
		void OnApplicationPause(bool paused) {
			if (!paused && tango && tango.IsServiceConnected) {
				// Tango service automatically disconnects when the app loses a focus.
				// So re-connect service when re-focused.
				Debug.Log("Unpaused. Reconnecting Tango service...");
				ConnectService();
			}
		}
		
		// If the name is blank or specified ADF is not found, null will be returned.
		AreaDescription FindAdf(string name) {
			if (name.Length == 0) {
				return null;
			} else {
				AreaDescription adf = TangoUtil.FindAdfByName(name);
				if (adf == null) {
					Debug.LogWarning("ADF not found; creating a new file.");
				} else {
					Debug.LogFormat("ADF found: \n Name: {0}, \n DateTime: {1}, \n UUID: {2}",
						adf.GetMetadata().m_name,
						adf.GetMetadata().m_dateTime,
						adf.m_uuid);
				}
				return adf;
			}
		}
		
		// Connect the service with requesting the permissions.
		// Fails connecting if the permissions is not granted along the way.
		void ConnectService() {
			Debug.Log("Attempting to connect Tango service...");
			
			StartCoroutine(_ConnectService());
		}

		// Connect the service, assuming all the required permissions have been granted.
		IEnumerator _ConnectService() {
			// Display Startup UI.
			startupNotification.OnIntended();

			// The thread will be busy running Shutdown() and Start().
			// Here ensures startup notification will be drawn/displayed beforehand.
			yield return null;

			if (!tango.m_areaDescriptionLearningMode && selectedADF == null) {
				// With this state, service would start completely blind.
				// Also we can't restart service from this state (for some reason).
				// We don't want it -- so don't start service.
				Debug.LogAssertion("Can't start service with this setup.");
			} else {
				yield return null;

				DisconnectService();

				// Just making sure.
				tango.m_enableMotionTracking = true;
				tango.m_enableAreaDescriptions = true;

				tango.Startup(selectedADF);
			}

			// Hide Startup UI.
			// This may not be invoked when the above procedure fails.
			startupNotification.OnStarted();
		}

		void DisconnectService() {
			if (!tango.IsServiceConnected) {
				Debug.LogWarning("Service not connected.");
			} else {
				tango.Shutdown();
			}
		}

		void SaveCurrentADF() {
			if (!tango.IsServiceConnected) {
				Debug.LogAssertion("Can't save ADF; service not connected.");
			} else if (!tango.m_areaDescriptionLearningMode) {
				Debug.LogAssertion("Can't save ADF; service not learning");
			} else {
				Debug.Log("Saving ADF...");
				
				string adfName = (adfNameText)
					? adfNameText.text
					: null;

				// Saves the currently learning ADF to the disk.
				// The progress is displayed by `OnTangoEventAvailableEventHandler` (above).
				tango.SaveCurrentADF(adfName, () => {
					if (reconnectOnSave) {
						// Restart service after saving (it won't automatically.)
						ConnectService();
					}
				});
			}
		}
	}
}
