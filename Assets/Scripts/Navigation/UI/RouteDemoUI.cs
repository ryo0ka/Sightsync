using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Navigation.UI {
	public class RouteDemoUI : MonoBehaviour {

		[SerializeField]
		float verticalOffset;

		[SerializeField]
		float animationSpeed;

		[SerializeField]
		float maxDistance;

		[SerializeField]
		float intervalSeconds;

		[SerializeField]
		LineRenderer linePrototype;

		[SerializeField]
		Button startButton;

		[SerializeField]
		Button addButton;

		[SerializeField]
		Button endButton;

		[SerializeField]
		Button runRouteButton;

		List<LineRenderer> lines;

		LineRenderer currentLine;

		void Start() {
			lines = new List<LineRenderer>();

			startButton.onClick.AddListener(StartNewRoute);
			addButton.onClick.AddListener(AddPoint);
			endButton.onClick.AddListener(EndRoute);
			runRouteButton.onClick.AddListener(RunRoute);
		}

		void StartNewRoute() {
			EndRoute();

			currentLine = Instantiate(linePrototype, linePrototype.transform.parent);
			currentLine.gameObject.SetActive(true);

			AddPoint();
		}

		void AddPoint() {
			if (currentLine) {
				int index = currentLine.numPositions++;
				Vector3 position = Camera.main.transform.position;
				position.y += verticalOffset;
				currentLine.SetPosition(index, position);
			}
		}

		void EndRoute() {
			if (currentLine) {
				lines.Add(currentLine);
				currentLine = null;
			}
		}

		void RunRoute() {
			if (lines.Count > 0) {
				LineRenderer last = lines[lines.Count - 1];
				Route route = last.gameObject.AddComponent<Route>();
				route.OffsetSpeed = animationSpeed;
				route.MaxDistance = maxDistance;
				route.IntervalSeconds = intervalSeconds;
				route.OnEnd += () => Destroy(route.gameObject);
			}
		}
	}
}
