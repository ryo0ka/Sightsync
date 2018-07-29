using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Assets.Ryooka.Scripts.EditorExtension;
using Assets.Ryooka.Scripts.Misc;

namespace Assets.Scripts.DriftCorrection {
	// TODO: Support smoothing drift correction.
	//       (Probably, make a dedicated class and make it a member.)
	public class DriftCorrector: MonoBehaviour {
		[Serializable]
		public struct Correction {
			public float height;
			public float radius;
		}

		class CorrectionType: BlurField<Correction>.ValueType {
			public static CorrectionType instance = new CorrectionType();

			public Correction Identity() {
				return new Correction();
			}

			public Correction Multiply(Correction v1, float time) {
				return new Correction { height = v1.height * time };
			}

			public Correction Sum(Correction v1, Correction v2) {
				return new Correction { height = v1.height + v2.height };
			}

			public Correction Subtract(Correction v1, Correction v2) {
				return Sum(v1, Multiply(v2, -1));
			}

			public Correction Blend(Correction v1, Correction v2) {
				return new Correction { height = (v1.height + v2.height) / 2 };
			}

			public float Radius(Correction v) {
				return v.radius;
			}
		}

		// Internal container class that helps serialize the field.
		[Serializable]
		class CorrectionGrid {
			public IntVector2 p;
			public Correction c;
		}
		
		// Internal container class that helps serialize the field.
		[Serializable]
		class State {
			public IntVector2 size;
			public int blur;
			public float scale;

			[HideInInspector]
			public List<CorrectionGrid> grids;
		}
		
		[Serializable]
		class StateFile : JsonDataFile<State> { }
		
		TangoARPoseController player;

		[SerializeField]
		[UnfoldInInspector]
		State state;

		[SerializeField]
		[UnfoldInInspector]
		StateFile file;

		BlurField<Correction> field;
		
		IntVector2 currentFieldPosition;

		// True if correction has been applied.
		// If false, correction should be applied in the next frame.
		public bool Corrected { get; private set; }

		// True if the defined data have not changed since field has constructed.
		// If false, field may consist of outdated data (thus should not be referred to).
		public bool FieldUpdated { get; private set; }

		// Width and depth of each grid in metric.
		public float FieldScale {
			get { return state.scale; }
		}

		public event Action<IntVector2> onPlayerPositionChanged = delegate { };

		// Unity function.
		void Start() {
			player = FindObjectOfType<TangoARPoseController>();

			if (player == null) {
				Debug.LogAssertion("TangoARPoseController not found");
			}

			Construct();
		}

		// Unity function.
		void Update() {
			ObserveCurrentGridPosition();

			if (!Corrected) {
				Correct();
			}
		}

		void ObserveCurrentGridPosition() {
			Vector3 globalPosition = player.transform.position;
			IntVector2 newGridPosition = WorldToGridPosition(globalPosition);

			// If the target has moved from a grid to another.
			if (newGridPosition != currentFieldPosition) {
				currentFieldPosition = newGridPosition;
				onPlayerPositionChanged(newGridPosition);
				Corrected = false;
			}
		}

		public void Correct() {
			//NOTE Removed global offset from here without testing.

			//NOTE Commented out local offset (below) to not use custom class in SDK. Not tested.
			//// Local offset (drift correction)
			//Correction c = Get(currentFieldPosition);
			//player.m_positionOffset.y += c.height;

			Corrected = true;
		}

		public void Construct() {
			field = new BlurField<Correction>(CorrectionType.instance, state.size);

			foreach (CorrectionGrid grid in state.grids) {
				field.DefineGrid(grid.p.x, grid.p.y, grid.c);
			}

			field.MapOutPredefinedGrids();
			field.Blur(state.blur);

			Corrected = false;
			FieldUpdated = true;
		}
		
		public void Set(IntVector2 position, Correction correction) {
			// Duplicate definition should be removed from the list.
			int duplicateIndex = state.grids.FindIndex(g => g.p == position);
            if (duplicateIndex >= 0) {
				state.grids.RemoveAt(duplicateIndex);
			}

			// add new user-defiend grid to state.
			state.grids.Add(new CorrectionGrid {
				p = position,
				c = correction,
			});

			Corrected = false;
			FieldUpdated = false;
		}

		public void Remove(IntVector2 position) {
			int foundIndex = state.grids.FindIndex(g => g.p == position);
			if (foundIndex >= 0) {
				state.grids.RemoveAt(foundIndex);

				Corrected = false;
				FieldUpdated = false;
			}
		}

		public void RemoveAll() {
			state.grids.Clear();

			Corrected = false;
			FieldUpdated = false;
		}
		
		// State of field must be accessed via this method.
		public Correction Get(IntVector2 position) {
			if (FieldUpdated) {
				return field.GetValue(position);
            } else {
				return GetDefined(position) 
					?? CorrectionType.instance.Identity();
            }
		}

		public Correction? GetDefined(IntVector2 position) {
			CorrectionGrid grid = state.grids.FirstOrDefault(g => g.p == position);
			if (grid == null) {
				return null;
			} else {
				return grid.c;
			}
        }

		// Must be enumerated through before the next frame.
		public IEnumerable<IntVector2> Grids() {
			return field.Grids();
		}

		public IntVector2 WorldToGridPosition(Vector3 globalPosition) {
			return new IntVector2(
				(int)(globalPosition.x / state.scale + state.size.x / 2),
				(int)(globalPosition.z / state.scale + state.size.y / 2));
		}

		public Vector3 GridToWorldPosition(IntVector2 gridPosition) {
			float x = state.scale * (gridPosition.x - state.size.x / 2);
			float y = state.scale * (gridPosition.y - state.size.y / 2);
			return new Vector3(x, 0, y);
		}

		public string Serialize() {
			return JsonUtility.ToJson(state);
		}

		public void Deserialize(string json) {
			state = JsonUtility.FromJson<State>(json);
		}
	}
}
