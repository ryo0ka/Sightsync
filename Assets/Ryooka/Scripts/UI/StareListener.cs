using System;
using UnityEngine;
using UnityEngine.Events;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Ryooka.Scripts.UI {
	/*  Observes how long a given camera stares at this GameObject's collider.
        If it's stared at for a given length of time, executes given events.
     */
	public class StareListener: MonoBehaviour {
		[Serializable]
		public class TriggerTimeListener : UnityEvent<float> { }

		public Camera targetCamera;
		public Collider targetCollider;

		public float maxStaringTime;
		public float maxHoldingTime;

		public UnityEvent onStaredAt;
		public UnityEvent onLookedAway;
		public TriggerTimeListener onBuilding; // 0f to 1f.

		StareState state;

		void Reset() {
			targetCamera = Camera.main;
			maxStaringTime = 1f;
			maxHoldingTime = 1f;
		}

		void Start() {
			state = new StareState();
		}

		void Update() {
			state.Update(
				isLookedAt: targetCollider.IsLookedAtBy(targetCamera), 
				staredTimeMax: maxStaringTime,
				holdingTimeMax: maxHoldingTime,
				listen:	Listen);
		}

		void Listen(StareState.State before, StareState.State after, float staring, float holding) {
			if (after == StareState.State.ON) {
				onBuilding.Invoke(staring / maxStaringTime);
			}
			if (before != StareState.State.AT && after == StareState.State.AT) {
				onStaredAt.Invoke();
				onBuilding.Invoke(0); //TODO good to be here?
			}
			if (before != StareState.State.AWAY && after == StareState.State.AWAY) {
				onLookedAway.Invoke();
			}
			if (before != StareState.State.OFF && after == StareState.State.OFF) {
				onBuilding.Invoke(0); //TODO good to be here?
			}
		}
	}

	public class StareState {
		public enum State { OFF, ON, AWAY, AT }

		public State previous { get; private set; }
		public State current { get; private set; }
		public float staring { get; private set; }
		public float holding { get; private set; }

		//TODO the current specification of "holding" is useless
		public void Update(bool isLookedAt, float staredTimeMax, float holdingTimeMax) {
			previous = current;
			if (previous == State.AT && isLookedAt) {
				// pass
			} else if (previous == State.AT && !isLookedAt) {
				current = State.AWAY;
				holding = 0;
			} else if (previous == State.AWAY && isLookedAt) {
				current = State.AT;
				holding = 0;
			} else if (previous == State.AWAY && !isLookedAt) {
				holding += Time.deltaTime;
				if (holding >= holdingTimeMax) {
					current = State.OFF;
					holding = 0;
				}
			} else if (previous == State.ON && isLookedAt) {
				staring += Time.deltaTime;
				if (staring >= staredTimeMax) {
					current = State.AT;
					staring = 0;
				}
			} else if (previous == State.ON && !isLookedAt) {
				current = State.OFF;
				staring = 0;
			} else if (previous == State.OFF && isLookedAt) {
				current = State.ON;
				staring = 0;
				holding = 0;
			} else if (previous == State.OFF && !isLookedAt) {
				// pass
			} else {
				// should not happen.
				throw new InvalidOperationException();
			}
		}

		public void Update(bool isLookedAt, float staredTimeMax, float holdingTimeMax, Action<State, State, float, float> listen) {
			Update(isLookedAt, staredTimeMax, holdingTimeMax);
			listen(previous, current, staring, holding);
		}

		public void ResetState() {
			current = previous = State.OFF;
			staring = holding = 0;
		}
	}
}
