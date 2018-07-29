using System;
using UnityEngine;

namespace Assets.Scripts.Navigation {
	// Modify the size of the clip plane
	// when the player is in a collision box
	// and is looking into another collision box.
	//
	// TODO Update cycle and some states should be managed by another script attached to Camera.
	[RequireComponent(typeof(Collider))]
	public class ClipPlaneEntity: MonoBehaviour {
		public ClipPlaneManager manager;

		public Collider targetArea;
		public float clipDepth;

		// Maximum distance from the camera to the clip plane.
		// Without this setting, it clips way too much stuff
		// when a player looks at a far point of the wall.
		// Also for the "let player walk" purpose we want this.
		public float maxClippingPlane;

		// Speed of expansion/shrinking of the clip plane.
		public float clipTransitionSpeed;

		// If true, this script can manipulate the clip plane depth.
		// Otherwise the default depth should be set.
		bool triggerOn;

		bool triggerOnPrev;

		float CurrentClipPlaneDepth {
			get { return manager.playerCamera.nearClipPlane; }
			set { manager.playerCamera.nearClipPlane = value; }
		}

		float DefaultDepth {
			get { return manager.defaultDepth; }
		}
		
		void Reset() {
			// Attempt to find PixelLightManager in the scene.
			manager = FindObjectOfType<ClipPlaneManager>();
		}

		void OnTriggerEnter(Collider collider) {
			if (IsPlayer(collider)) {
				triggerOn = true;
				Debug.Log("Entered: " + name);
			}
		}

		void OnTriggerExit(Collider collider) {
			if (IsPlayer(collider)) {
				triggerOn = false;
				Debug.Log("Exited: " + name);
			}
		}

		void Update() {
			float clipPlanePrev = CurrentClipPlaneDepth;
			float clipPlaneNext = clipPlanePrev;

			if (triggerOn) {
				RaycastHit hitInfo;

				// "10" is a general max distance. should be parametererized?
				if (targetArea.Raycast(PlayerForwardRay(), out hitInfo, 10)) {
					// distance between the camera and the wall
					float distance = hitInfo.distance;

					// near plane should be a sum of
					// the distance from the camera to the wall
					// and the desired depth from the wall to the inside.
					float clipLength = distance + clipDepth;
					
					// if too long, shrink it to the specified maximum length.
					clipPlaneNext = Mathf.Min(clipLength, maxClippingPlane);
				} else {
					// If looking away, retrieve the clip plane back to default
					clipPlaneNext = DefaultDepth;
				}
			} else if (triggerOnPrev) {
				// if trigger from ON to OFF (i.e. this script should stop working),
				// retrieve the clip plane back to default
				clipPlaneNext = DefaultDepth;
			}

			if (clipPlaneNext != clipPlanePrev) {
				// transition of clip plane
				float delta = clipPlaneNext - clipPlanePrev;
				float timedDelta = delta * Time.deltaTime * clipTransitionSpeed;

				CurrentClipPlaneDepth += (delta >= 0)
					? Mathf.Min(delta, timedDelta)
					: Mathf.Max(delta, timedDelta);
			}
		
			triggerOnPrev = triggerOn;
		}

		Ray PlayerForwardRay() {
			return new Ray {
				origin = manager.playerCamera.transform.position,
				direction = manager.playerCamera.transform.forward,
			};
		}

		bool IsPlayer(Collider collider) {
			return collider == manager.playerCollider;
		}


	}
}
