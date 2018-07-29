using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Ryooka.Scripts.EditorExtension;
using Assets.Ryooka.Scripts.Extension;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.Misc {
	public class DaidarabotchiEffect: MonoBehaviour {
		[Serializable]
		public class EffectElement {
			[UnfoldInInspector]
			[SerializeField]
			Animation animation;

			bool direction;
			float time;

			public void Update(bool newDirection) {
				if (direction != newDirection) {
					direction = newDirection;
					time = 0;
				}
				time += Time.deltaTime;
				animation.SetState(
					time: time,
					forward: direction);
				animation.Play();
			}

			public Vector3 Position() {
				return animation.transform.position;
			}
		}

		public float maxLength;
		public float maxTime;
		public Transform center;
		public List<EffectElement> elements;

		bool playing;
		bool direction;
		float time;

		void Update() {
			if (!playing) return;
			time += Time.deltaTime;
			if (time.OutOfBounds(0, maxTime)) {
				playing = false;
				return;
			}
			float length = MathR.Lerp(
				min: (direction) ? 0 : maxLength,
				max: (direction) ? maxLength : 0, 
				time: time / maxTime,
				type: MathR.LerpType.LINEAR);
			if (length.OutOfBounds(0, maxLength)) {
				playing = false;
				return;
			}
			UpdateElements(length);
		}

		void UpdateElements(float length) {
			foreach (var e in elements)
				if (Activated(length, e.Position()))
					e.Update(direction);
		}

		bool Activated(float length, Vector3 position) {
			float distance = Vector3.Distance(
				a: position,
				b: center.position);
			return (direction && distance < length)
			   || (!direction && distance > length);
        }

		public void Play(bool newDirection) {
			playing = true;
			//if (newDirection == direction) return;
			direction = newDirection;
			time = 0;
		}
	}
}
