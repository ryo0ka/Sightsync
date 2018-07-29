using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Lightmap {
	[Serializable]
	public class LightmapDataSnapshot {
		public Texture2D color;
		public Texture2D shadowMask;

		public LightmapData ToLightmapData() {
			return new LightmapData {
				lightmapColor = color,
				shadowMask = shadowMask,
			};
		}
	}
}
