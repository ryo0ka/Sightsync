using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.Extension {
	public static class RendererUtil {
		public static void SetColor(this Renderer self, float? r = null, float? b = null, float? g = null, float? a = null) {
			Color color = self.material.color;
			float _r = r.GetValueOrDefault(color.r);
			float _b = b.GetValueOrDefault(color.b);
			float _g = g.GetValueOrDefault(color.g);
			float _a = a.GetValueOrDefault(color.a);
			self.material.color = new Color(_r, _g, _b, _a);
		}

		public static void SetAlpha(this Renderer self, float value) {
			foreach (var material in self.materials) {
				var color = material.color;
				color.a = value;
				material.color = color;
			}
		}

		public static IEnumerator Fade(this Renderer self, float start, float end, float time) {
			foreach (var value in MathR.LerpE(start, end, time)) {
				self.SetAlpha(value);
				yield return null;
			}
		}

		public static void SetRenderQueue(this Renderer self, int queue) {
			foreach (var material in self.materials) {
				material.renderQueue = queue;
			}
		}

		public static void SetRenderQueue(this Renderer self, int[] orders) {
			Material[] materials = self.materials;
			for (int i = 0; i < materials.Length && i < orders.Length; ++i) {
				materials[i].renderQueue = orders[i];
			}
		}

		public static void RenderAsBackground(this Renderer self) {
			self.SetRenderQueue(0);
		}

		public static IEnumerable<Material> GetMaterials(this GameObject self) {
			return self.GetComponents<Renderer>(-1, true).SelectMany(r => r.materials);
		}

		public static void CopyLightMapSettings(this Renderer self, Renderer original) {
			self.lightmapIndex = original.lightmapIndex;
			self.lightmapScaleOffset = original.lightmapScaleOffset;
			self.realtimeLightmapIndex = original.realtimeLightmapIndex;
			self.realtimeLightmapScaleOffset = original.realtimeLightmapScaleOffset;
		}
	}
}
