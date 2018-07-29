using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.UI {
	/*
	SETTING UP CAMERA:
		Set the camera as targetCamera of this script.
		Set: targetCamera > Depth        > Whatever larger than that of Main Camera.
		Set: targetCamera > Clear Flags  > "Solid Color". (linked as [1])
		Set: targetCamera > Background   > A > "0".       (linked as [1])
	
	SETTING UP RAWIMAGE:
		A RawImage component must be present.
		Set the image as targetImage of this script.
		Expand the component to the full screen.

	LINKS:
		[1] http://answers.unity3d.com/questions/24085/something-about-render-texture-transparent.html	
	*/

	[ExecuteInEditMode]
	public class CameraFeed: MonoBehaviour {
		// Specify which camera will render into RenderTexture.
		// This is not Tango AR Camera.
		public Camera targetCamera;

		// Speficy which component will render AR overlay on screen.
		// Must be placed full screen (or have the same aspect ratio as the screen).
		public RawImage targetImage;

		// You must invoke InitializeTexture() to apply this change.
		public bool supportStencilBuffer = true;

		public Toggle activeToggle;

		// Serialized so that rendering works on Editor.
		[HideInInspector]
		[SerializeField]
		RenderTexture texture;
		
		virtual protected void Start() {
			InitializeTexture();

			if (activeToggle) {
				activeToggle.onValueChanged.AddListener(SetActive);
			}
		}

		void OnEnable() {
			SetActive(true);
		}

		void OnDisable() {
			SetActive(false);
		}

		public void InitializeTexture() {
			// Texture's resolution is decided by image's size.
			// Screen resolution can be used too but it's unnecessarily high.
			// Assuming the image has the same aspect ratio as the screen.
			Rect res = targetImage.rectTransform.rect;
			int width  = (int)res.width;
			int height = (int)res.height;

			Debug.Log("Texture resolution: " + width + "x" + height);
			Debug.Log("Screen resolution:  " + Screen.width + "x" + Screen.height);

			// "24" is to support stencil buffer. Otherwise set 16.
			int depth = (supportStencilBuffer) ? 24 : 16;

			// Instantiate texture.
			texture = new RenderTexture(width, height, depth);
			texture.name = "Camera Feed Texture (" + gameObject.name + ")";
			texture.Create();

			// If texture is already assigned (when playing on editor),
			// discard it to prevent memory leaks.
			if (targetCamera.targetTexture) {
				targetCamera.targetTexture.Release();
			}

			// Let targetCamera render into texture.
			targetCamera.targetTexture = texture;

			// Let targetImage render texture.
			targetImage.texture = texture;
		}
		
		public void SetActive(bool active) {
			if (targetCamera)
				targetCamera.enabled = active;

			if (targetImage)
				targetImage.enabled = active;

			if (activeToggle) {
				activeToggle.isOn = active;
			}
		}
	}
}
