using UnityEngine;

namespace Assets.Ryooka.Scripts.Extension {
	public static class StandardUtil {
		public enum BlendMode {
			Opaque,
			Cutout,
			Fade,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
			Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
		}

		public const string BLEND_MODE = "_Mode";
		public const string TAG_RENDER_TYPE = "RenderType";
		public const string RENDERTYPE_OPAQUE = "";
		public const string RENDERTYPE_TRANSPARENT_CUTOUT = "TransparentCutout";
		public const string RENDERTYPE_TRANSPRANET ="Transparent";
		public const string SRC_BLEND = "_SrcBlend";
		public const string DST_BLEND = "_DstBlend";
		public const string Z_WRITE = "_ZWrite";
		public const string EMISSION_COLOR = "_EmissionColor";
		public const string COLOR = "_Color";
		public const string KEY_EMISSION  = "_EMISSION";
		public const string KEY_ALPHATEST = "_ALPHATEST_ON";
		public const string KEY_ALPHABLEND = "_ALPHABLEND_ON";
		public const string KEY_ALPHAPREMULTIPLY = "_ALPHAPREMULTIPLY_ON";

		public static bool IsOfStandardShader(this Material material) {
			return material.HasProperty(COLOR)
				&& material.HasProperty(EMISSION_COLOR);
		}

		public static void SetColor(this Material m, Color color) {
			m.SetColor(COLOR, color);
		}

		public static Color GetEmissionColor(this Material m) {
			return m.GetColor(EMISSION_COLOR);
		}

		// Do pass null to `emissionColor` when using Standard Shader.
		public static void SetEmissionColor(this Material m, Color color) {
			m.EnableKeyword(KEY_EMISSION);
			m.SetColor(EMISSION_COLOR, color);
		}

		public static void SetActiveKeyword(this Material self, string keyword, bool enabled) {
			if (enabled) {
				self.EnableKeyword(keyword);
			} else {
				self.DisableKeyword(keyword);
			}
		}

		public static BlendMode GetBlendMode(this Material material) {
			return (BlendMode)material.GetFloat(BLEND_MODE);
		}

		// assuming Unity Standard Shader
		public static void SetBlendMode(this Material material, BlendMode blendMode) {
			material.SetFloat(BLEND_MODE, (float)blendMode);

			switch (blendMode) {
				case BlendMode.Opaque:
					material.SetOverrideTag(TAG_RENDER_TYPE, RENDERTYPE_OPAQUE);
					material.SetInt(SRC_BLEND, (int)UnityEngine.Rendering.BlendMode.One);
					material.SetInt(DST_BLEND, (int)UnityEngine.Rendering.BlendMode.Zero);
					material.SetInt(Z_WRITE, 1);
					material.DisableKeyword(KEY_ALPHATEST);
					material.DisableKeyword(KEY_ALPHABLEND);
					material.DisableKeyword(KEY_ALPHAPREMULTIPLY);
					material.renderQueue = -1;
					break;
				case BlendMode.Cutout:
					material.SetOverrideTag(TAG_RENDER_TYPE, RENDERTYPE_TRANSPARENT_CUTOUT);
					material.SetInt(SRC_BLEND, (int)UnityEngine.Rendering.BlendMode.One);
					material.SetInt(DST_BLEND, (int)UnityEngine.Rendering.BlendMode.Zero);
					material.SetInt(Z_WRITE, 1);
					material.EnableKeyword(KEY_ALPHATEST);
					material.DisableKeyword(KEY_ALPHABLEND);
					material.DisableKeyword(KEY_ALPHAPREMULTIPLY);
					material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
					break;
				case BlendMode.Fade:
					material.SetOverrideTag(TAG_RENDER_TYPE, RENDERTYPE_TRANSPRANET);
					material.SetInt(SRC_BLEND, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					material.SetInt(DST_BLEND, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					material.SetInt(Z_WRITE, 0);
					material.DisableKeyword(KEY_ALPHATEST);
					material.EnableKeyword(KEY_ALPHABLEND);
					material.DisableKeyword(KEY_ALPHAPREMULTIPLY);
					material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
					break;
				case BlendMode.Transparent:
					material.SetOverrideTag(TAG_RENDER_TYPE, RENDERTYPE_TRANSPRANET);
					material.SetInt(SRC_BLEND, (int)UnityEngine.Rendering.BlendMode.One);
					material.SetInt(DST_BLEND, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					material.SetInt(Z_WRITE, 0);
					material.DisableKeyword(KEY_ALPHATEST);
					material.DisableKeyword(KEY_ALPHABLEND);
					material.EnableKeyword(KEY_ALPHAPREMULTIPLY);
					material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
					break;
			}
		}
	}
}
