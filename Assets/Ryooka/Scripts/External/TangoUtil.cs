#if true
using System;
using System.Threading;
using System.Collections;
using System.Linq;
using UnityEngine;
using Tango;

namespace Assets.Ryooka.Scripts.External {
	public static class TangoUtil {
		const string ADF_SAVE_PROGRESS = "AreaDescriptionSaveProgress";
		const string SERVICE_EXCEPTION = "TangoServiceException";
		const string FISHEYE_OVER_EXPOSED = "FisheyeOverExposed";
		const string FISHEYE_UNDER_EXPOSED = "FisheyeUnderExposed";
		const string COLOR_OVER_EXPOSED = "ColorOverExposed";
		const string COLOR_UNDER_EXPOSED = "ColorUnderExposed";
		const string TOO_FEW_FEATURES_TRACKED = "TooFewFeaturesTracked";

		// Retrieves the latest named ADF found in the disk.
		// Null if the permission is granted but the file is not found.
		// The behavior is undefined when a user permission has not been granted.
		public static AreaDescription FindAdfByName(string name) {
			return AreaDescription.GetList()
				.Where(d => d.GetMetadata().m_name == name) // Matches name.
				.OrderBy(n => n.GetMetadata().m_dateTime) // Sorts by date & time.
				.LastOrDefault(); // Null if not found.
		}

		// Saves current ADF.
		// A coroutine is set to run on TangoApplication.
		// Should not be executed when permissions have not been granted or service is not running.
		public static void SaveCurrentADF(this TangoApplication self, string name = null, Action onComplete = null) {
			self.StartCoroutine(SaveCurrentADF(name, onComplete));
		}

		public static IEnumerator SaveCurrentADF(string name, Action onComplete) {
			bool doneSaving = false;
			Exception error = null;

			new Thread(() => {
				try {
					// Save ADF.
					var currentADF = AreaDescription.SaveCurrent();

					// If name is speficied, name the ADF.
					if (name != null) currentADF.SaveNameAs(name);
				} catch (Exception e) {
					error = e;
				} finally {
					doneSaving = true;
				}
			}).Start();

			while (!doneSaving) {
				yield return null;
			}

			if (error != null) {
				Debug.LogAssertion(error);
			}

			if (onComplete != null) {
				onComplete();
			}
		}

		public static void SaveNameAs(this AreaDescription self, string name) {
			var metadata = self.GetMetadata();
			metadata.m_name = name;
			self.SaveMetadata(metadata);
		}

		public static float? GetADFSaveProgress(this TangoEvent self) {
			if (self.type == TangoEnums.TangoEventType.TANGO_EVENT_AREA_LEARNING
			 && self.event_key == ADF_SAVE_PROGRESS) {
				return float.Parse(self.event_value) * 100;
			} else {
				return null;
			}
		}

		// True if the service just localized.
		// This is "correct", reliable pose data.
		// If true, motion tracking is working properly.
		public static bool IsLocalized(this TangoPoseData self) {
			return self.framePair.baseFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION
				&& self.framePair.targetFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_START_OF_SERVICE;
		}

		// True if motion tracking is working properly.
		// Not necessarily "correct"; may contain some drifting.
		public static bool IsTracking(this TangoPoseData self) {
			return self.framePair.baseFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION
				&& self.framePair.targetFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_DEVICE;
		}

		public static bool IsInitializing(this TangoPoseData self) {
			return self.status_code == TangoEnums.TangoPoseStatusType.TANGO_POSE_INITIALIZING;
		}

		public static bool IsValid(this TangoPoseData self) {
			return self.status_code == TangoEnums.TangoPoseStatusType.TANGO_POSE_VALID;
		}

		public static bool IsInvalid(this TangoPoseData self) {
			return self.status_code == TangoEnums.TangoPoseStatusType.TANGO_POSE_INVALID;
		}

		// Makes sure to restart the service properly.
		// If the service hasn't started, just starts the service normally.
		public static void Restart(this TangoApplication self, AreaDescription adf) {
			self.Shutdown();
			self.Startup(adf);
		}
	}
}
#endif

