using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.UI {
	// Synchronizes registered Toggles' ON/OFF state.
	// Doesn't support editor-time operation.
	// Don't register too many in a lifetime.
	// To assign "global" actions, register a Toggle with no graphic property.
	public class ToggleIdentity : MonoBehaviour {
		[Serializable]
		public class BoolEvent : UnityEvent<bool> { }
		
		class ToggleEvent {
			public Toggle toggle { get; private set; }

			public ToggleEvent(Toggle toggle, Func<IEnumerable<ToggleEvent>> toggles, Func<bool> active) {
				this.toggle = toggle;
				toggle.onValueChanged.AddListener(value => {
					if (!active()) return;
					var allToggles = toggles();
					if (allToggles.Contains(this))
						foreach (var t in allToggles)
							t.toggle.isOn = value;
				});
			}
		}

		[SerializeField]
		public bool active;

        [SerializeField]
		List<Toggle> targetToggles;
		
		[NonSerialized]
		List<ToggleEvent> toggleEvents;

		void Reset() {
			active = true;
		}

		void Awake() { //if doesn't work, try Start
			toggleEvents = new List<ToggleEvent>();
			RegisterAll();
		}

		void OnValidate() {
			if (toggleEvents == null) return; //for pre-runtime
			RegisterAll();
		}

		void RegisterAll() {
			targetToggles.ForEach(RegisterToggle);
		}

		void RegisterToggle(Toggle toggle) {
			if (toggle == null) return; //for inspector
			if (HasRegistered(toggle)) return;
			toggleEvents.Add(new ToggleEvent(
				toggle: toggle,
				toggles: ActiveToggleEvents,
				active: IsActive));
		}

		bool HasRegistered(Toggle t) {
			return toggleEvents.Where(te => te.toggle == t).Count() != 0;
		}

		IEnumerable<ToggleEvent> ActiveToggleEvents() {
			return toggleEvents.Where(te => targetToggles.Contains(te.toggle));
		}

		bool IsActive() {
			return enabled && active;
		}
	}
}
