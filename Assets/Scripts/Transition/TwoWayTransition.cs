using System;
using UnityEngine;

namespace Assets.Scripts.Transition {
	[Serializable]
	public class TwoWayTransition {
		enum State { TO_PING, TO_PONG, AT_PING, AT_PONG, }

		[Range(0.1f, 30f)]
		public float duration;

		[Range(0f, 1f)]
		public float _position;

		float startTime;

		State stateLast, stateNow;
		
		TwoWayTransition() {
			duration = .5f;
			startTime = 0f;
			stateLast = stateNow = State.AT_PONG;
		}
		
		//  forward : 0 -> 1
		// !forward : 0 <- 1
		public void Update(bool forward, out float position, out bool done) {
			stateNow = (forward) ? State.TO_PING : State.TO_PONG;

			float progress = Mathf.Clamp01((Time.time - startTime) / duration);

			bool _done = false;

			if (FromTo(State.AT_PONG, State.TO_PING)) {
				// initialize the "up" animation
				startTime = Time.time;
			} else if (FromTo(State.AT_PING, State.TO_PONG)) {
				// initialize the "down" animation
				startTime = Time.time;
			} else if (FromTo(State.TO_PONG, State.TO_PING)) {
				progress = ReverseProgress(progress);
			} else if (FromTo(State.TO_PING, State.TO_PONG)) {
				progress = ReverseProgress(progress);
			} else if (FromTo(State.TO_PING, State.TO_PING)) {
				if (progress >= 1f) {
					stateNow = State.AT_PING;
					_done = true;
				}
			} else if (FromTo(State.TO_PONG, State.TO_PONG)) {
				if (progress >= 1f) {
					stateNow = State.AT_PONG;
					_done = true;
				}
			} else if (FromTo(State.AT_PING, State.TO_PING)) {
				// Fix the state
				stateNow = State.AT_PING;
				_done = true;
			} else if (FromTo(State.AT_PONG, State.TO_PONG)) {
				// Fix the state
				stateNow = State.AT_PONG;
				_done = true;
			}

			stateLast = stateNow;

			position = _position = (forward) ? progress : (1f - progress);

			done = _done;
		}

		bool FromTo(State _stateLast, State _stateNow) {
			return stateLast == _stateLast && stateNow == _stateNow;
		}

		float ReverseProgress(float progress) {
			float progressReverse = 1f - progress;
			startTime += (progress - progressReverse) * duration;
			return progressReverse;
		}
	}
}
