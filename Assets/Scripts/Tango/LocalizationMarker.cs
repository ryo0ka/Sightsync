using System.Collections.Generic;
using UnityEngine;
using Tango;
using Assets.Ryooka.Scripts.External;

namespace Assets.Scripts.Tango { 
	// I couldn't watch out TangoSpecUI's localization count every moment,
	// so decided to just mark wherever has been localized.
	// This way it'd be obvious where I should go to and learn.
	// Polycount is a concern, so make sure the prototype's mesh is simple.
	//
	// TODO: This shit is not dealing with drifting when learning. Implement the math.
	public class LocalizationMarker : MonoBehaviour, ITangoPose {
		// Minimum distance between indicators (in metric).
		// So we won't generate zillions of markers colliding each other.
		[SerializeField]
		float minimumDistance;

		[SerializeField]
		Transform markerPrefab;

		List<Transform> markers;
		Transform player;

		void Start() {
			markers = new List<Transform>();

			TangoApplication tango = FindObjectOfType<TangoApplication>();
			tango.Register(this);

			player = FindObjectOfType<TangoARPoseController>().transform;
		}

		void OnEnable() {
			if (markers != null) {
				SetActiveMarkers(true);
			}
		}

		void OnDisable() {
			if (markers != null) {
				SetActiveMarkers(false);
			}
		}

		void ITangoPose.OnTangoPoseAvailable(TangoPoseData poseData) {
			// Don't do anything when not supposed to.
			if (!isActiveAndEnabled) return;

			if (poseData.IsLocalized()) {
				Vector3 p = player.position;
				bool tooClose = false;

				foreach (var m in markers) {
					Vector3 _p = m.position;
					
					if (Vector3.Distance(_p, p) < minimumDistance) {
						tooClose = true;
						break;
					}
				}

				if (!tooClose) {
					var parent = markerPrefab.parent;
                    var newM = Instantiate(markerPrefab, parent);
					markers.Add(newM);
				}
			}
		}

		public void ClearMarkers() {
			markers.Clear();
		}

		void SetActiveMarkers(bool visible) {
			foreach (var m in markers) {
				m.gameObject.SetActive(visible);
			}
		}
	}
}
