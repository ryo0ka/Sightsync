using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Transition {
	public class DateTransitionManager: MonoBehaviour {
		[Serializable]
		public sealed class Token {
			public DateTransitionManager manager;

			// Name shared with an Entity in the manager.
			public string entryName;

			public Action<bool> OnSetVisible;

			public void Register() {
				manager.tokens.Add(this);
			}
		}

		[Serializable]
		public sealed class Entry {
			public string name;

			// the year this object was constructed
			public int yearAppeared;

			// the year this object was demolished
			public int yearDisappeared;
		}

		public bool startFromLatestYear;

		// sliders of year.
		public List<Slider> yearSliders;

		public List<Entry> entries;

		HashSet<Token> tokens;

		int minYear, maxYear;

		DateTransitionManager() : base() {
			tokens = new HashSet<Token>();
		}

		void Start() {
			UpdateYearBounds();

			foreach (var yearSlider in yearSliders) {
				yearSlider.onValueChanged.AddListener(floatYear => {
					SetYear((int)floatYear);
				});
			}

			if (startFromLatestYear) {
				SetYear(maxYear);
			}
		}

		public void UpdateYearBounds() {
			minYear = int.MaxValue;
			maxYear = int.MinValue;

			foreach (Entry e in entries) {
				if (e.yearAppeared < minYear) {
					minYear = e.yearAppeared;
				}
				if (e.yearDisappeared > maxYear) {
					maxYear = e.yearDisappeared;
				}
			}

			foreach (var yearSlider in yearSliders) {
				yearSlider.minValue = minYear;
				yearSlider.maxValue = maxYear;
			}
		}

		public void SetYear(int year) {
			foreach (Entry e in entries) {
				// hide objects that were not yet born or were demolished at the date, and
				// show objects that were both born and still alive at the date.
				bool visible = e.yearAppeared <= year && year <= e.yearDisappeared;

				foreach (Token t in tokens) {
					if (t.entryName == e.name) {
						t.OnSetVisible(visible);
					}
				}
			}

			foreach (var yearSlider in yearSliders) {
				yearSlider.value = year;
			}
		}
	}
}
