using Assets.Scripts.Navigation.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UserTest {
	public class Exhibition: MonoBehaviour {
		public abstract class DisplayContent: MonoBehaviour {
			public abstract void SetVisible(bool visible);

			public void Show() {
				SetVisible(true);
			}

			public void Hide() {
				SetVisible(false);
			}
		}

		[Serializable]
		class DisplayItem {
			public DisplayContent display;
			public StandBox standBox;
		}

		[SerializeField]
		bool visible;

		[SerializeField]
		Collider player;

		[SerializeField]
		Button forceExitButton;

		[SerializeField]
		GameObject finishNotification;

		[SerializeField]
		PositionIndicator positionIndicator;

		[SerializeField]
		List<DisplayItem> items;

		int currentIndex;

		// Unity function
		void OnValidate() {
			if (!Application.isPlaying) {
				SetVisibleAll(visible);
			}
		}

		// Unity function
		void Start() {
			SetVisibleAll(false);
			forceExitButton.onClick.AddListener(() => {
				forceExitButton.gameObject.SetActive(false);
				AdvanceItem();
			});
			InitializeEvents();
		}

		void InitializeEvents() {
			foreach (var item in items) {
				item.standBox.SetPlayer(player);
				item.standBox.ClearEvents();
				item.standBox.OnEnter(() => {
					item.display.Show();
					forceExitButton.gameObject.SetActive(true);
					positionIndicator.Hide();
                });
				//item.standBox.OnExit(item.display.Hide);
			}
			UpdateState();
		}

		void SetVisibleAll(bool visible) {
			this.visible = visible;
			foreach (var item in items) {
				item.display.SetVisible(visible);
			}
		}

		void AdvanceItem() {
			currentIndex += 1;

			if (currentIndex >= items.Count) {
				currentIndex = 0;
				finishNotification.SetActive(true);
			}

			UpdateState();
		}

		void UpdateState() {
			foreach (var item in items) {
				item.standBox.gameObject.SetActive(false);
				item.display.Hide();
			}

			var currentItem = items[currentIndex];
			currentItem.standBox.gameObject.SetActive(true);
			Debug.LogFormat("Exhibition::UpdateState - currentBox = {0}", currentItem.standBox.name);

			positionIndicator.ActivateWithTarget(currentItem.standBox.transform);
        }
	}
}
