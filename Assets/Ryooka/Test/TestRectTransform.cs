using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Ryooka.Scripts.Test {
	public class TestRectTransform : MonoBehaviour {
		public RectTransform rect;

		float y {
			get {
				return rect.localPosition.y;
			}
			set {
				var p = rect.localPosition;
				p.y = value;
				rect.localPosition = p;
			}
		}

		void OnGUI() {
			GUILayout.BeginArea(new Rect(0, 0, 200, 100));
			GUILayout.Label(y.ToString());
			y = GUILayout.HorizontalSlider(y, y-50, y+50);
			GUILayout.Label(rect.NormalizedLocalPosition().ToString());
			GUILayout.EndArea();
		}
	}
}
