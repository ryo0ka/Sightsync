using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.Extension {
    [RequireComponent(typeof(Text))]
    public class TextExt : MonoBehaviour {
		[Header("Set 0 if not substring.")]
		public int substring;

        private Text text {
            get { return GetComponent<Text>(); }
        }

        public void SetText(int v) {
            text.text = v.ToString();
        }

        public void SetText(float v) {
			var t = v.ToString();
			if (substring > 0 && t.Length > substring)
				t = t.Substring(0, substring);
			text.text = t;
        }

        public void SetText(Vector3 v) {
            text.text = v.ToString();
        }

		public void SetText(GameObject v) {
			text.text = v.name;
		}
    }
}
