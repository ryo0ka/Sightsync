using System;
using System.Collections;
using UnityEngine;
using Assets.Ryooka.Scripts.General;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Ryooka.Scripts.UI {
	[ExecuteInEditMode]
	public class ShiftScreen: MonoBehaviour {
		static readonly float TOP_MIN =  1f/2;
		static readonly float TOP_MAX =  3f/2;
		static readonly float BTM_MIN = -1f/2;
		static readonly float BTM_MAX =  1f/2;

		public RectTransform screen;
		public RectTransform topPanel;
		public RectTransform bottomPanel;

		[Range(0, 1)]
		public float position;

		public float shiftTime;

		public MathR.LerpType shiftType;

		public bool StartWithTopPanel;

		[SerializeField]
		[HideInInspector]
		float position_last; //for direct modification of `position`

		void Start() {
			if (Application.isPlaying && StartWithTopPanel) {
				SetPosition(0);
			}
		}

		void Update() {
			// Reflects a change of `position` if not reflected yet.
			if (position != position_last) {
				SetPosition(position);
				position_last = position;
			}
		}

		public void SetPosition(float normalized) {
			var topPosN = MathR.Map(normalized, 0, 1, TOP_MIN, TOP_MAX);
			var btmPosN = MathR.Map(normalized, 0, 1, BTM_MIN, BTM_MAX);
			topPanel.SetNormalizedLocalPosition(topPosN.WithX(.5f), screen);
			bottomPanel.SetNormalizedLocalPosition(btmPosN.WithX(.5f), screen);
			position = position_last = normalized;
		}

		void SwapPanels() {
			GeneralR.Swap(ref topPanel, ref bottomPanel);
			SetPosition(1 - position); //TODO what if `position` is outside 0-1?
		}

		//Shifts from top to bottom and swaps them.
		IEnumerator _ShiftPanels(float time) {
			foreach (var pos in MathR.LerpE(0f, 1f, time, shiftType)) {
				if (pos < position) continue; //starts from the current location
				SetPosition(pos);
				yield return null;
			}
			SwapPanels();
		}

		public void ShiftPanels() {
			StartCoroutine(_ShiftPanels(shiftTime));
		}
	}
}
