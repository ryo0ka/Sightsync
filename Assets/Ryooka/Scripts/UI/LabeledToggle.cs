using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.UI {
	public class LabeledToggle : Toggle {
		Text label {
			get { return GetComponentInChildren<Text>(); }
		}

		public string text {
			get { return label.text; }
			set { label.text = value; }
		}
	}
}
