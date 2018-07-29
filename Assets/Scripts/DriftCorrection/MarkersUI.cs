using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.DriftCorrection {
	public class MarkersUI: MonoBehaviour {
		[SerializeField]
		Button addButton;

		[SerializeField]
		Button removeButton;

		[SerializeField]
		Transform markerPrefab;

		[NonSerialized]
		Stack<Transform> markers;

		void Start() {
			markers = new Stack<Transform>();

			addButton.onClick.AddListener(SpawnNewMarker);
			removeButton.onClick.AddListener(DespawnLastMarker);
		}

		public void SpawnNewMarker() {
			Transform marker = Instantiate(
				original: markerPrefab,
				parent: markerPrefab.parent);
			
			marker.position = Camera.main.transform.position;
			marker.rotation = Camera.main.transform.rotation;

			// The prototype object may have been deactivated
			// (to not increase random prefabs in the project.)
			marker.gameObject.SetActive(true);

			markers.Push(marker);

			Debug.Log("New marker was instantiated.");
		}

		public void DespawnLastMarker() {
			if (markers.Count > 0) {
				Transform lastMarker = markers.Pop();
				Destroy(lastMarker.gameObject);
			} else {
				Debug.Log("No markers are alive.");
			}
		}
	}
}
