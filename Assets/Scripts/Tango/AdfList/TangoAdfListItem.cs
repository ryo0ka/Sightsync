using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Tango.AdfList {
	public class TangoAdfListItem : MonoBehaviour {
		[SerializeField]
		Button selectButton;

		[SerializeField]
		Text nameText;

		[SerializeField]
		Text dateText;

		[SerializeField]
		Text uuidText;

		public void SetName(string name) {
			nameText.text = name;
		}

		public void SetUUID(string uuid) {
			uuidText.text = uuid;
		}

		public void SetTime(DateTime time) {
			dateText.text = time.ToShortDateString();
		}

		public void OnSelected(UnityAction onSelected) {
			selectButton.onClick.AddListener(onSelected);
		}
	}
}
