using System;
using UnityEngine;

namespace Assets.Ryooka.Scripts.General {
	[Serializable]
	public class TransformSnapshot {
		public static TransformSnapshot zero = new TransformSnapshot(Vector3.zero, Quaternion.identity);

		[SerializeField]
		[HideInInspector]
		Vector3 position; //immutable

		[SerializeField]
		[HideInInspector]
		Quaternion rotation; //immutable

		public Vector3 Position { get { return position; } }
		public Quaternion Rotation { get { return rotation; } }

		public TransformSnapshot(Vector3 position, Quaternion rotation) {
			this.position = position;
			this.rotation = rotation;
		}

		public TransformSnapshot(Transform transform)
			: this(transform.position, transform.rotation) { }
		
		public void Substitute(Transform transform) {
			transform.position = position;
			transform.rotation = rotation;
		}

		public Vector3 Forward() {
			return rotation * Vector3.forward;
		}

		public override int GetHashCode() {
			return position.GetHashCode() + rotation.GetHashCode(); //TODO
		}

		public override bool Equals(object obj) {
			TransformSnapshot that = obj as TransformSnapshot;
			return (that != null) && Equals(that);
		}

		bool Equals(TransformSnapshot that) {
			return this.position == that.position
				&& this.rotation == that.rotation;
		}

		public override string ToString() {
			return string.Format("TransformSnapshot({0}, {1})", position, rotation);
		}
	}
}
