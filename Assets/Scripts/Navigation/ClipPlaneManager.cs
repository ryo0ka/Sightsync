using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Navigation {
	public class ClipPlaneManager : MonoBehaviour {
		public float defaultDepth;

		public Camera playerCamera;
		public Collider playerCollider;

		// Keep track of set counts, so that the highest count is always used.
		// This is used as a queue.
		List<ClipPlaneEntity> activeEntities;

		void Awake() {
			activeEntities = new List<ClipPlaneEntity>();

			// Initialize the clip depth with defaultDepth when the game starts.
			// This may be redundant with OnValidate() but just making sure it must work.
			SetClipPlaneDepth(defaultDepth);
		}

		// Unity function; Invoked when any values are modified in inspector.
		void OnValidate() {
			if (!Application.isPlaying) {
				// Apply defaultDepth to playerCamera.
				// Skip this if the game is playing.
				SetClipPlaneDepth(defaultDepth);
			}
		}

		public void ActivateEntity(ClipPlaneEntity entity) {
			activeEntities.Add(entity);

			SetClipPlaneDepth(entity.clipDepth);
		}

		public void DeactivateEntity(LightingEntity entity) {
			activeEntities.RemoveAll(entity.Equals);

			// Avoid setting the count to defaultDepth when entities are nested.
			SetClipPlaneDepth((activeEntities.Count > 0)
				? activeEntities.Last().clipDepth
                : defaultDepth);
		}

		void SetClipPlaneDepth(float depth) {
			playerCamera.nearClipPlane = depth;
		}
	}
}
