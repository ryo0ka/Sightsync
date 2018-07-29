using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tango;
using Assets.Ryooka.Scripts.Extension;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts.Tango.AdfList {
	public class TangoAdfListUI : MonoBehaviour {
		[SerializeField]
		Transform container;

		[SerializeField]
		TangoAdfListItem itemPrototype;

		[SerializeField]
		Button reloadButton;

		[SerializeField]
		GameObject loadingScreen;

		public event Action<AreaDescription> onAdfSelected;

		bool reloading;

		void Start() {
			if (reloadButton) {
				reloadButton.onClick.AddListener(() => {
					ReloadList();
				});
			}
		}

		public void ReloadList() {
			StartCoroutine(_ReloadList());
		}

		IEnumerator _ReloadList() {
			if (reloading) {
				// Don't reload twice at the same time
				yield break;
			}

			try {
				Debug.Log("Loading ADF list...");

				reloading = true;

				loadingScreen.SetActive(true);

				// Make sure loading screen is displayed
				yield return new WaitForSeconds(0.5f);

				// clear the list
				foreach (var item in GetListedItems()) {
					if (item != itemPrototype) {
						Destroy(item.gameObject);
					}
				}

				// the first item is "null" that means to create a new ADF
				{
					TangoAdfListItem item = AddNewItem();

					item.SetName("(Create a new ADF)");

					item.OnSelected(() => onAdfSelected(null));
				}

				// construct items from existing ADFs
				foreach (var _adf in GetAllSortedADFs()) {
					// foreach is stupid
					AreaDescription adf = _adf;

					// spawn an item in the list
					TangoAdfListItem item = AddNewItem();

					// construct UI of item
					item.SetUUID(adf.m_uuid);
					item.SetName(adf.GetMetadata().m_name);
					item.SetTime(adf.GetMetadata().m_dateTime);

					// when item is clicked, adf will be selected.
					item.OnSelected(() => onAdfSelected(adf));
				}
			} finally {
				reloading = false;

				loadingScreen.SetActive(false);

				Debug.Log("Finished loading ADF list...");
			}
		}

		TangoAdfListItem AddNewItem() {
			// Instantiate itemPrototype as a child object of container.
			var item = Instantiate(itemPrototype, container);

			// itemPrototype may be an inactive scene object, so activate it.
			item.gameObject.SetActive(true);

			return item;
		}

		IEnumerable<TangoAdfListItem> GetListedItems() {
			return container.GetComponents<TangoAdfListItem>(1, false);
		}

		// Fetch the list of ADFs from the device sorted by their name.
		IEnumerable<AreaDescription> GetAllSortedADFs() {
			// The list is null when permissions haven't been granted,
			// so in that case, proceed with an empty list.
			// The service procudes a warning, so we don't warn it here.
			return (AreaDescription.GetList() ?? new AreaDescription[0])
				.OrderByDescending(adf => adf.GetMetadata().m_dateTime);
		}
	}
}
