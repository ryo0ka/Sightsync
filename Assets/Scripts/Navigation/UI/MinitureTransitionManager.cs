using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Ryooka.Scripts.General;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Scripts.Navigation.UI {
	// RULES:
	// - Manages a Miniture camera's and player's transform.
	// - Does NOT interact with or correspond to the game state,
	//   however, provides methods and utilities for one to perform such tasks.
	// - Does NOT manage or manipulate any transforms of the other objects in Miniture.
	// 
	// ASSUMPTIONS:
	// - Miniture and its "original" object must share the same imported transformation.
	// - Said objects also must share the same transformation in their local coordinate system.
	public class MinitureTransitionManager : MonoBehaviour {
		// Object of the player (camera).
		// This object will be used to display the player's tranform in Miniture.
		public Transform player;

		// Object of the player (camera) in the scene.
		// This script uses the global coord position of this object as the position of the player.
		public Camera mainCamera;

		// Camera obejct in Miniture.
		public Camera minitureCamera;

		// Camera to determine where minitureCamera should be positioned to provide a top view. 
		// topViewCamera can be disabled anytime (because its input is not used).
		// topViewCamera must be in the same coordinate system as minitureCamera.
		public Camera topViewCamera;

		// Player object in Miniture.
		public Transform miniturePlayer;

		// An object inside playerIndicator
		// which will be used to display the player's rotation in the global coords.
		public Transform minitureDevice;

		// Maximum scale of miniturePlayer.
		// Adjust this until miniturePlayer object has a proper size in Top View.
		public float playerScaleInTopView;

		// Used for the sin animation.
		public float playerMaxVerticalPosition;

		// Name of the color property of the shader(s) used by renderers of miniturePlayer.
		public string playerColorPropertyName;

		// Time length in seconds that this script's transtion animation should take.
		[Range(0.1f, 10f)]
		public float transitionSeconds;

		[Range(0f, 10f)]
		public float playerSinSeconds;
		
		public AnimationCurve topCameraFovTranslation;
		public AnimationCurve topCameraPositionTranslation;
		public AnimationCurve topCameraRotationTranslation;
		public AnimationCurve topCameraPlayerScaleTranslation;
		public AnimationCurve playerAppearingTranslation;

		// Debuggers
		public Button topViewTransitionButton;
		public Button sceneViewSyncButton;

		// miniturePlayer's materials, used for changing the transparency.
		List<Material> miniturePlayerMaterials;

		// While true, newly prompted animation may not be registered.
		// There can be up to one animation running simultaneously.
		public bool IsTransitioning { get; set; }

		void Start() {
			// Get all the materials used by miniturePlayer.
			miniturePlayerMaterials = miniturePlayer.gameObject.GetMaterials().ToList();
			
			if (topViewTransitionButton) {
				topViewTransitionButton.onClick.AddListener(StartTransitionToTopCamera);
			}

			if (sceneViewSyncButton) {
				sceneViewSyncButton.onClick.AddListener(SynchronizeToMainCamera);
			}
		}

		// temporary
		void Update() {
			// Sync the miniture player's transform to the actual player object's.
			miniturePlayer.localPosition = player.position;
			minitureDevice.localRotation = player.rotation;
		}

		public void StartTransitionToTopCamera() {
			StartCoroutine(_TransitionToTopCamera());
		}

		// Imediately sync minitureCamera's transform and FOV to mainCamera.
		public void SynchronizeToMainCamera() {
			minitureCamera.fieldOfView = mainCamera.fieldOfView;
			minitureCamera.transform.localPosition = mainCamera.transform.position;
			minitureCamera.transform.localRotation = mainCamera.transform.rotation;
			SetMiniturePlayerTransparency(0f);
		}

		IEnumerator _TransitionToTopCamera() {
			Debug.Log("Top view transitioning...");

			// Do not interrupt other animations.
			if (IsTransitioning) {
				Debug.Log("Animation already running.");
				yield break;
			}

			// Notify that this animation is ongoing and should not be interrupted.
			IsTransitioning = true;
			
			// Initial FOV of minitureCamera.
			// This is used to make the transition work properly.
			float initMinitureCameraFov = minitureCamera.fieldOfView;

			// Initial position of minitureCamera.
			// This is used to make the transition work properly.
			Vector3 initMinitureCameraPos = minitureCamera.transform.localPosition;

			// Initial rotation of minitureCamera.
			// This is used to make the transition work properly.
			Quaternion initMinitureCameraRot = minitureCamera.transform.localRotation;

			// Store the transition's progress at the last frame.
			// Used for preventing jitters.
			float startedTime = Time.time;

			// Time at which the transition completed.
			// Used for offsetting miniturePlayer's sin animation.
			// Also used to flag the completion (-1 means incomplete).
			float completedTime = -1;

			while (true) {
				if (completedTime >= 0 && IsTransitioning) {
					// If completed and IsTransitioning is on,
					// that means another animation is pending while this animatino is done.
					// Then end this animation completely.
					yield break;
				}

				// Sync the miniture player's transform to the actual player object's.
				miniturePlayer.localPosition = player.position;
				minitureDevice.localRotation = player.rotation;

				// Normalized time of the progress used for Mathf.Lerp() function.
				// (Using Time.time is stabler than Time.deltaTime.)
				float t = (Time.time - startedTime) / transitionSeconds;

				// Until t is 1 (that is transitionSeconds), run the transition.
				if (t <= 1) {
					// sync the Field of view gradually.
					minitureCamera.fieldOfView = Mathf.Lerp(
						a: initMinitureCameraFov,
						b: topViewCamera.fieldOfView,
						t: topCameraFovTranslation.Evaluate(t));

					// Move minitureCamera to mainCamera's reflected position gradually
					minitureCamera.transform.localPosition = Vector3.Lerp(
						a: initMinitureCameraPos,
						b: topViewCamera.transform.localPosition,
						t: topCameraPositionTranslation.Evaluate(t));

					// Rotates minitureCamera.
					minitureCamera.transform.localRotation = Quaternion.Lerp(
						a: initMinitureCameraRot,
						b: topViewCamera.transform.localRotation,
						t: topCameraRotationTranslation.Evaluate(t));

					// Scale up miniturePlayer so it's visible in a far view.
					miniturePlayer.localScale = Vector3.Lerp(
						a: Vector3.one,
						b: playerScaleInTopView.To3(),
						t: topCameraPlayerScaleTranslation.Evaluate(t));

					// Make miniturePlayer appear gradually.
					SetMiniturePlayerTransparency(Mathf.Lerp(
						a: 0f,
						b: 1f,
						t: playerAppearingTranslation.Evaluate(t)));
				} else if (completedTime < 0) {
					Debug.Log("Top view transition completed.");

					// Notify that this animation can halt anytime.
					IsTransitioning = false;

					completedTime = Time.time;
				} else {
					// If playerSinSeconds is 0 (to not animate miniturePlayer), don't animate.
					if (playerSinSeconds == 0) {
						// do nothing
					} else {
						// normalized time of the sin animation.
						float time = (Time.time - completedTime) / playerSinSeconds;

						// the displacement should depend on the local coords' scale,
						// so that this animation won't let the avatar jump into the sky.
						float scale = playerMaxVerticalPosition * miniturePlayer.localScale.y;

						// Apply the animation.
						// This algorithm only works when the position is initialized every frame.
						float verticalPosition = Mathf.Sin(time * scale);
						Vector3 localPosition = miniturePlayer.localPosition;
						localPosition.y += verticalPosition;
						miniturePlayer.localPosition = localPosition;
					}
				}

				yield return null;
			}
		}

		void SetMiniturePlayerTransparency(float alpha) {
			foreach (Material m in miniturePlayerMaterials) {
				Color playerColor = m.GetColor(playerColorPropertyName);
				playerColor.a = alpha;
				m.SetColor(playerColorPropertyName, playerColor);
			}
		}
	}
}
