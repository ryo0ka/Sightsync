using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace Assets.Ryooka.Scripts.General {
	public class PerFrameSampler<T> {
		LinkedList<T> self; // for head/tail ops
		int capacity;
		Func<T> defaultValue;
		Func<T> sample;

		public PerFrameSampler(int capacity, Func<T> sample, Func<T> defaultValue) {
			this.capacity = capacity;
			this.defaultValue = defaultValue;
			this.sample = sample;
			self = new LinkedList<T>();
		}
		
		public void Update() {
			self.AddFirst(sample());
			while (self.Count > capacity)
				self.RemoveLast();
		}

		T ElementAt(int index) {
			try {
				return self.ElementAt(index);
			} catch (ArgumentOutOfRangeException) {
				return defaultValue();
			}
		}

		public T PreviousFrame(int frame) {
			return ElementAt(frame);
		}

		public T PreviousFrame() {
			return ElementAt(0);
		}

		//https://docs.unity3d.com/ScriptReference/Time-deltaTime.html
		public T PreviousTime(float seconds) {
			return ElementAt(TimeToFrame(seconds));
		}

		float FrameToTime(int frame) {
			return frame * Time.deltaTime;
		}

		int TimeToFrame(float time) {
			return (int)(time / Time.deltaTime);
		}

		public int RecordedFrameCount() {
			return self.Count();
		}

		public float RecordedTimeLength() {
			return FrameToTime(RecordedFrameCount());
		}
	}
}
