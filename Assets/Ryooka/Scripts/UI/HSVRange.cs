using UnityEngine;
namespace Assets.Ryooka.Scripts.UI {
	// https://github.com/greggman/hsva-unity/
	public static class HSVRange {
		// Property name of the HSVA value (defined in the shader).
		const string _HSVAAdjust = "_HSVAAdjust";

		public static Vector4 GetHSVAAdjust(Material mat) {
			return mat.GetVector(_HSVAAdjust);
		}

		public static void SetHSVAAdjust(Material mat, Vector4 hsva) {
			mat.SetVector(_HSVAAdjust, hsva);
		}

		public static float GetHue(Material mat) {
			return GetHSVAAdjust(mat).x + 1f;
		}

		public static void SetHue(Material mat, float hue) {
			Vector4 hsva = GetHSVAAdjust(mat);
			hsva.x = Mathf.Clamp01(hue) - 1f;
			SetHSVAAdjust(mat, hsva);
		}

		public static float GetSaturation(Material mat) {
			return (GetHSVAAdjust(mat).y + 1f) * 2f;
		}

		public static void SetSaturation(Material mat, float saturation) {
			Vector4 hsva = GetHSVAAdjust(mat);
			hsva.y = (Mathf.Clamp01(saturation) - 1f) * 0.5f;
			SetHSVAAdjust(mat, hsva);
		}

		public static float GetValue(Material mat) {
			return GetHSVAAdjust(mat).z + 1f;
		}

		public static void SetValue(Material mat, float value) {
			Vector4 hsva = GetHSVAAdjust(mat);
			hsva.z = Mathf.Clamp01(value) - 1f;
			SetHSVAAdjust(mat, hsva);
		}

		public static float GetAlpha(Material mat) {
			return GetHSVAAdjust(mat).w + 1f;
		}

		public static void SetAlpha(Material mat, float alpha) {
			Vector4 hsva = GetHSVAAdjust(mat);
			hsva.w = Mathf.Clamp01(alpha) - 1f;
			SetHSVAAdjust(mat, hsva);
		}
	}
}
