using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.Extension {
	public static class UIUtil {
		public static void SetHandleToCenter(this Slider self, float range, float value) {
			self.minValue = value - range / 2;
			self.maxValue = value + range / 2;
			self.value = value;
		}

		public static void SetHandleToCenter(this Slider self, float value) {
			self.SetHandleToCenter(self.maxValue - self.minValue, value);
		}

		public static void SetCurrentValueToCenter(this Slider self) {
			self.SetHandleToCenter(self.value);
		}
		
		public static float CenterValue(this Slider self) {
			return (self.maxValue - self.minValue) / 2 + self.minValue;
		}

		public static void SetColor(this RawImage self, float? r = null, float? g = null, float? b = null, float? a = null) {
			Color color = self.color;
			if (r.HasValue) color.r = r.Value;
			if (g.HasValue) color.g = g.Value;
			if (b.HasValue) color.b = b.Value;
			if (a.HasValue) color.a = a.Value;
			self.color = color;
		}
	}
}
