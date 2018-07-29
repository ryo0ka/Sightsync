using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.Extension {
	[RequireComponent(typeof(InputField))]
	public class InputFieldExt : MonoBehaviour {
		InputField inputField {
			get { return GetComponent<InputField>(); }
		}

		public void CloneTextFrom(Text from) {
			inputField.text = from.text;
		}
	}
}
