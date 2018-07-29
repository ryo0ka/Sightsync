using Tango;
using UnityEngine;
using UnityEngine.UI;
using Assets.Ryooka.Scripts.External;

namespace Assets.Scripts.Tango {
	public class TangoInspectorUI: MonoBehaviour, ITangoLifecycle, ITangoPose, ITangoEventMultithreaded {
		
		public Toggle toggleValid;
		public Toggle togglePoseBaseADF;
		public Toggle togglePoseBaseSOS;
		public Toggle togglePoseTargetSOS;
		public Toggle togglePoseTargetDVC;
		public Toggle toggleConnection;
		public Toggle togglePermission;
		public Text textLocalizationCount;
		public Button buttonResetLocalizationCount;
		public Text adfSaveProgressText;
		public Text eventText;

		TangoApplication app;

		int localizationCount;

		bool savingThisFrame;
		float saveProgress;

		bool eventUpdated;
		string eventMessage;

		void Start() {
			app = FindObjectOfType<TangoApplication>();

			app.Register(this);

			buttonResetLocalizationCount.onClick.AddListener(ResetLocalizationCount);
		}

		void OnDestroy() {
			app.Unregister(this);
		}

		void Update() {
			if (eventUpdated) {
				if (savingThisFrame) {
					adfSaveProgressText.text = saveProgress + " %";
					savingThisFrame = false;
				}

				eventText.text = eventMessage;

				eventUpdated = false;
			}
		}

		void ITangoPose.OnTangoPoseAvailable(TangoPoseData poseData) {
			if (isActiveAndEnabled) {
				toggleValid.isOn = poseData.IsValid();
				togglePoseBaseADF.isOn = poseData.framePair.baseFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION;
				togglePoseBaseSOS.isOn = poseData.framePair.baseFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_START_OF_SERVICE;
				togglePoseTargetSOS.isOn = poseData.framePair.targetFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_START_OF_SERVICE;
				togglePoseTargetDVC.isOn = poseData.framePair.targetFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_DEVICE;
			
				if (poseData.IsLocalized()) {
					textLocalizationCount.text = ++localizationCount + "";
				}
			}
		}

		void ITangoEventMultithreaded.OnTangoEventMultithreadedAvailableEventHandler(TangoEvent tangoEvent) {
			eventUpdated = true;

			eventMessage = string.Format("{0} - {1} : {2}", 
				tangoEvent.type, tangoEvent.event_key, tangoEvent.event_value);

			// handle event for ADF save progress
			float? progress = tangoEvent.GetADFSaveProgress();
			if (progress.HasValue) {
				savingThisFrame = true;
				saveProgress = progress.Value;
			}
		}

		void ResetLocalizationCount() {
			localizationCount = 0;
			textLocalizationCount.text = 0 + "";
        }

		void ITangoLifecycle.OnTangoPermissions(bool permissionsGranted) {
			togglePermission.isOn = permissionsGranted;
		}

		void ITangoLifecycle.OnTangoServiceConnected() {
			toggleConnection.isOn = true;
		}

		void ITangoLifecycle.OnTangoServiceDisconnected() {
			toggleConnection.isOn = false;
			togglePermission.isOn = false;
		}
	}
}
