using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Navigation.UI;

namespace Assets.Scripts.Navigation {
	// Manages the state/event transition of the tour.
	public class Tour: MonoBehaviour {
		// (For now) The state will flow based on an array of path names and events.
		// An element of the array contains the name of the path and
		// events that must be invoked when the specified path is completed.
		// The element is a standalone MonoBehavior that is specified in inspector.

		[Serializable]
		public class TourElement {
			public string pathName;
			public TourEvent eventSpec;
		}

		public LineRenderer pathPrototype;

		public RoutePoint routePointPrototype;

		public Button startTourButton;

		public Button skipCurrentEventButton;

		public InputField fileNameInputField;

		public string fileName;

		public List<TourElement> tourEvents;

		public TourEvent startUpEvent;

		public TourEvent endingEvent;
		
		RouteDataSet settings;

		int currentTourIndex;

		TourEvent currentEvent;

		Route currentRoute;
		
		void Start() {
			if (startTourButton) {
				startTourButton.onClick.AddListener(() => {
					StartTour();
				});
			}

			if (fileNameInputField) {
				fileNameInputField.onEndEdit.AddListener(t => {
					fileName = t;
				});
			}

			if (skipCurrentEventButton) {
				skipCurrentEventButton.onClick.AddListener(() => {
					currentRoute.Halt();
				});
			}
		}

		public void StartTour() {
			currentTourIndex = -1;

			settings = RouteSerializer.Instance.Load(fileName);

			InvokeCurrentElement();
		}
		
		void InvokeCurrentElement() {
			if (currentTourIndex < 0) {
				Debug.Log("Initializing a tour.");

				// If starting the tour, invoke the initial event.
				// The initial event must finish completely.
				startUpEvent.InvokeEvent(onComplete: () => {
					currentTourIndex = 0;
					InvokeCurrentElement();
				});
			} else if (currentTourIndex == tourEvents.Count) {
				Debug.Log("Ending a tour.");

				// If ending the tour, invoke the ending event.
				endingEvent.InvokeEvent();

				currentTourIndex += 1;
			} else if (currentTourIndex > tourEvents.Count) {
				Debug.Log("No more events found.");
			} else {
				Debug.Log("Proceeding a tour.");

				if (currentRoute != null) {
					// If the previous route is still active, halt it.
					currentRoute.Halt();
				}

				if (currentEvent != null) {
					// If the previous event is still running, halt it.
					currentEvent.Halt();
				}

				TourElement elem = tourEvents[currentTourIndex];
				RouteData source = settings.routes.FirstOrDefault(s => s.name == elem.pathName);

				currentEvent = elem.eventSpec;

				if (source == null) {
					Debug.LogError("Route not found! Route name: " + elem.pathName + ", File name: " + fileName);
					
					// If the current route is not found, go on to the next one (just so it's not done).
					currentTourIndex += 1;
					InvokeCurrentElement();
				} else if (source.points.Length == 0) {
					Debug.LogWarning("Path length zero: " + source.name + ". Skipping.");

					// If the current route is not found, go on to the next one (just so it's not done).
					currentTourIndex += 1;
					InvokeCurrentElement();
				} else {
					LineRenderer path = Instantiate(pathPrototype, pathPrototype.transform.parent);
					path.gameObject.SetActive(true);

					// Construct LineRenderer.

					path.positionCount = source.points.Length;

					for (int i = 0; i < source.points.Length; i++) {
						path.SetPosition(i, source.points[i]);
					}

					// Construct Route.

					currentRoute = path.gameObject.AddComponent<Route>();

					currentRoute.OffsetSpeed = settings.offsetSpeed;
					currentRoute.IntervalSeconds = settings.intervalSeconds;
					currentRoute.MaxDistance = source.maxDistance;

					// Set up path/point interaction.

					RoutePoint endPoint = Instantiate(routePointPrototype, routePointPrototype.transform.parent);
					endPoint.gameObject.SetActive(true);
					endPoint.transform.position = source.points.Last();

					currentRoute.OnEnd += () => {
						// When route ends (that is when the player reaches the last position),
						// invoke the assigned event and proceed to the next path.
						
						currentEvent.InvokeEvent();
						endPoint.Finish(); // Kill the end point.

						currentTourIndex += 1;
						InvokeCurrentElement();
					};
				}
			}
		}
	}
}
