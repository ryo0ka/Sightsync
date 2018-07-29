using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.Sensor {
    public class WebCamera: MonoBehaviour {
        public RawImage display;

		[SerializeField]
		ScreenOrientation orientation;

		WebCamTexture texture;

		void OnValidate() {
			if (Application.isPlaying) {
				Screen.orientation = orientation;
			}
		}
		
        void Start() {
            texture = new WebCamTexture();
            display.texture = texture;
            display.material.mainTexture = texture;
            texture.Play();
            SetupCameraDisplay();
            foreach (var cam in WebCamTexture.devices) Debug.Log(cam.name);
        }
		
        void SetupCameraDisplay() {
            SetupFit();
			Screen.orientation = orientation;
        }

        void SetupFit() {
            var aspectFilter = display.gameObject.GetComponent<AspectRatioFitter>();
            if (aspectFilter != null && aspectFilter.IsActive()) {
                float h = texture.height;
                float w = texture.width;
                float ratio = w / h;
                Debug.LogFormat("Height: {0}, Width: {1}, Aspect Ratio: {2}", h, w, ratio);
                aspectFilter.aspectRatio = ratio;
            } else {
                Debug.LogWarningFormat(
                    "AspectRatioFitter is not set or activated. Screen may be stretched.");
            }
        }
    }
}
