using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.UI {
	public class LabeledButton : Button {
		Text label {
			get { return GetComponentInChildren<Text>(true); }
		}

		public string text {
			get { return label.text; }
			set { label.text = value; }
		}
    }
}
