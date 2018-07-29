using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.Misc {
	public class AverageTransform {
		public class Sample {
			public Vector3 position { get; private set; }
			public Quaternion rotation { get; private set; }

			public Sample(Vector3 position, Quaternion rotation) {
				this.position = position;
				this.rotation = rotation;
			}
		}

		int sampleCapacity;
		Queue<Sample> samples;

		public AverageTransform(int sampleCapacity) {
			this.sampleCapacity = Mathf.Max(0, sampleCapacity);
			samples = new Queue<Sample>();
		}

		public void Apply(Transform transform, int amount) {
			Sample smoothed = Smooth(transform, amount);
            transform.position = smoothed.position;
			transform.rotation = smoothed.rotation;
		}

		Sample Smooth(Transform sample, int amount) {
			return Smooth(new Sample(
				position: sample.position,
				rotation: sample.rotation), amount);
		}

		Sample Smooth(Sample sample, int amount) {
			AddSample(sample);
			return Smooth(amount);
        }

		Sample Smooth(int amount) {
			var samples = LatestSamples(amount);
			return new Sample(
				position: AverageVectors(samples.Select(s => s.position)),
				rotation: AverageQuaternion(samples.Select(s => s.rotation)));
		}

		void AddSample(Sample sample) {
			samples.Enqueue(sample);
			while (samples.Count > sampleCapacity)
				samples.Dequeue();
		}

		IEnumerable<Sample> LatestSamples(int amount) {
			var reverseAmount = samples.Count - amount;
			return samples.Where((_, i) => i >= reverseAmount);
		}

		Vector3 AverageVectors(IEnumerable<Vector3> vs) {
			return VectorR.Average(vs);
		}

		Quaternion AverageQuaternion(IEnumerable<Quaternion> qs) {
			return QuaternionR.Average(qs);
		}
	}
}
