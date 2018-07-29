using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Ryooka.Scripts.UI;

namespace Assets.Scripts.Navigation.UI {
	public class RouteConstructionUI : MonoBehaviour {
		// A route with temporary/runtime states (including UI components).
		class Token {
			public LineRenderer line;

			public LabeledToggle selector;

			// Temporary placeholder of vertical offset of the path.
			public float verticalOffset;

			// Temporary placeholder of Route::MaxDistance for serialization.
			// This property is unique to each route.
			public float maxDistance;
		}

		// Route shader's main color value.
		// Used to change the transparency to indicate selection.
		const string COLOR = "_EmissionColor";

		public Button addRouteButton;

		public Button removeRouteButton;

		public Button addPointButton;

		public Button undoButton;

		public Button restartLineButton;

		public Button startRouteButton;

		public Button haltRouteButton;

		public ToggleGroup selectorList;

		public LabeledToggle selectorPrototype;

		public LineRenderer linePrototype;

		public InputField routeNameInputField;

		public InputField fileNameInputField;

		public Button loadButton;

		public Button saveButton;

		public Slider offsetSpeedSlider;

		public Slider maxDistanceSlider;

		public Slider intervalSecondsSlider;

		public Slider verticalOffsetSlider;

		List<Token> tokens;
		Token currentSelected;
		Route currentRoute;

		// These are (usually) shared with all Routes
		// so should be managed here instead of in each RouteData.
		float offsetSpeed;
		float intervalSeconds = 0.1f;

		void Start() {
			tokens = new List<Token>();

			addRouteButton.onClick.AddListener(() => {
				Token token = AddNewToken();

				// Select the new token when created via the button.
				token.selector.isOn = true;
			});

			removeRouteButton.onClick.AddListener(() => {
				if (currentSelected != null) {
					Destroy(currentSelected);
					currentSelected = null;
				}
			});

			addPointButton.onClick.AddListener(() => {
				if (currentSelected != null) {
					int index = currentSelected.line.positionCount++;
					Vector3 position = Camera.main.transform.position;

					// apply offset position
					position.y += currentSelected.verticalOffset;

					currentSelected.line.SetPosition(index, position);
				}
			});

			undoButton.onClick.AddListener(() => {
				if (currentSelected != null) {
					currentSelected.line.positionCount = Mathf.Max(
						currentSelected.line.positionCount - 1, 0);
				}
			});

			if (restartLineButton) {
				restartLineButton.onClick.AddListener(() => {
					if (currentSelected != null) {
						currentSelected.line.positionCount = 0;
					}
				});
			}

			// Name of routes is in Token.selector.text.
			routeNameInputField.onEndEdit.AddListener(name => {
				if (currentSelected != null) {
					currentSelected.selector.text = name;
				}
			});

			loadButton.onClick.AddListener(() => {
				Load();
			});

			saveButton.onClick.AddListener(() => {
				Save();
			});

			startRouteButton.onClick.AddListener(() => {
				if (currentSelected == null || currentRoute) {
					// don't start this when currentRoute is set or nothing is selected.
					Debug.Log("Route is not selected or already animating.");
				} else {
					Debug.Log("Route started");

					// for making it a closure, we need a local variable
					Token selectedToken = currentSelected;

					// Route eats up a line's positions, 
					// so duplicate the current line to be sacrificed for it.
					LineRenderer duplicatedLine = Instantiate(selectedToken.line);

					// Copy the original line's positions to the duplicated line.
					duplicatedLine.positionCount = selectedToken.line.positionCount;
					for (int i = 0; i < selectedToken.line.positionCount; i++) {
						duplicatedLine.SetPosition(i, selectedToken.line.GetPosition(i));
					}

					// Instantiate Route with the duplicated line.
					Route route = duplicatedLine.gameObject.AddComponent<Route>();

					// Initialize the Route's properties.
					route.OffsetSpeed = offsetSpeed;
					route.IntervalSeconds = intervalSeconds;
					route.MaxDistance = selectedToken.maxDistance;

					// this is invoked either when completed or halted.
					route.OnEnd += () => {
						// Turn back on the original line/selector when route is done.
						selectedToken.line.gameObject.SetActive(true);

						// Re-select the original token when the route completes.
						selectedToken.selector.isOn = true;

						// delete duplicated instances.
						Destroy(route.gameObject);
					};

					// Turn off the original line/selector while route is active.
					// Otherwise they will sit on top of each other.
					selectedToken.line.gameObject.SetActive(false);

					// Expose this route so it can be halted anytime.
					currentRoute = route;
				}
			});

			haltRouteButton.onClick.AddListener(() => {
				HaltCurrentRoute();
			});

			offsetSpeedSlider.onValueChanged.AddListener(value => {
				offsetSpeed = value;

				if (currentRoute) {
					currentRoute.OffsetSpeed = value;
				}
			});

			maxDistanceSlider.onValueChanged.AddListener(value => {
				if (currentSelected != null) {
					currentSelected.maxDistance = value;
				}

				if (currentRoute) {
					currentRoute.MaxDistance = value;
				}
			});

			intervalSecondsSlider.onValueChanged.AddListener(value => {
				intervalSeconds = value;

				if (currentRoute) {
					currentRoute.IntervalSeconds = value;
				}
			});

			verticalOffsetSlider.onValueChanged.AddListener(value => {
				if (currentSelected != null) {
					// offset is determined by comparing the current input with the last input.
					// The last input is stored as Token::verticalOffset.
					float lastOffset = currentSelected.verticalOffset;
					Vector3 move = new Vector3(0, value - lastOffset, 0);
					currentSelected.verticalOffset = value;
					MoveAllPositions(currentSelected.line, move);

					if (currentRoute) {
						MoveAllPositions(currentRoute.Line, move);
					}
				}
			});
		}

		Token AddNewToken(string name = null) {
			// Clean up tokens with the same name.
			// This is done to prevent duplicates when loading the same set of routes from the disk.
			if (name != null) {
				Token duplicated = tokens.FirstOrDefault(t => t.selector.text == name);
				if (duplicated != null) {
					Destroy(duplicated);
				}
			}

			Token token = new Token {
				line = Instantiate(linePrototype, linePrototype.transform.parent),
				selector = Instantiate(selectorPrototype, selectorPrototype.transform.parent),
			};

			// acitvate all
			token.line.gameObject.SetActive(true);
			token.selector.gameObject.SetActive(true);

			// register group (so on/off is synched)
			token.selector.group = selectorList;

			token.selector.onValueChanged.AddListener(selected => {
				Select(token, selected);
			});

			if (name != null) {
				token.selector.text = name;
			}

			tokens.Add(token);

			return token;
		}

		void Select(Token token, bool selected) {
			if (!token.selector || !token.line) {
				// if broken, break 100%.
				Destroy(token);
			} else {
				// highlight selected lines
				Color c = token.line.material.GetColor(COLOR);
				c.r = (selected) ? 1f : 0f;
				c.g = (selected) ? 1f : 0f;
				c.b = (selected) ? 1f : 0f;
				token.line.material.SetColor(COLOR, c);

				if (selected) {
					// "Actually" select the given token.
					// Done here because otherwise the sliders in the next lines
					// would invoke their callbacks on the currently selected token.
					currentSelected = token;

					// Initialize sliders.
					maxDistanceSlider.value = token.maxDistance;
					verticalOffsetSlider.value = 0f;

					// While a Route is running, if other tokens are selected,
					// the route should halt immediately.
					// This is done to limit corner cases that are difficult to manage.
					// This input is pointless anyway.
					HaltCurrentRoute();
				}
			}
		}

		void Destroy(Token token) {
			Destroy(token.line.gameObject);
			Destroy(token.selector.gameObject);
			tokens.Remove(token);
		}

		void HaltCurrentRoute() {
			if (currentRoute) {
				// This invokes Route::onEnd.
				currentRoute.Halt();
			}
		}

		void Load() {
			HaltCurrentRoute(); // just in case.

			RouteDataSet s = RouteSerializer.Instance.Load(fileNameInputField.text);

			offsetSpeed = s.offsetSpeed;
			intervalSeconds = s.intervalSeconds;

			foreach (RouteData route in s.routes) {
				// deserialize name
				Token token = AddNewToken(route.name);

				// deserialzie lines
				token.line.positionCount = route.points.Length;
				for (int i = 0; i < route.points.Length; i++) {
					token.line.SetPosition(i, route.points[i]);
				}

				// deserialize route properties
				token.maxDistance = route.maxDistance;
			}

			// initialize sliders.
			offsetSpeedSlider.value = offsetSpeed;
			intervalSecondsSlider.value = intervalSeconds;
		}

		void Save() {
			HaltCurrentRoute(); // just in case.

			RouteData[] routes = tokens.Select(t => {
				// serialize lines
				Vector3[] points = new Vector3[t.line.positionCount];
				for (int i = 0; i < t.line.positionCount; i++) {
					points[i] = t.line.GetPosition(i);
				}
				return new RouteData {
					points = points,
					name = t.selector.text,
					maxDistance = t.maxDistance,
				};
			}).ToArray();

			RouteDataSet s = new RouteDataSet {
				routes = routes,
				offsetSpeed = offsetSpeed,
				intervalSeconds = intervalSeconds,
			};

			RouteSerializer.Instance.Save(s, fileNameInputField.text);
		}

		//TODO should be an extension method
		static void MoveAllPositions(LineRenderer line, Vector3 vector) {
			for (int i = 0; i < line.positionCount; ++i) {
				Vector3 pos = line.GetPosition(i);
				pos += vector;
				line.SetPosition(i, pos);
			}
		}
	}
}
