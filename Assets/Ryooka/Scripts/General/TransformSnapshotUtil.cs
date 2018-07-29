using Assets.Ryooka.Scripts.Extension;
using UnityEngine;

namespace Assets.Ryooka.Scripts.General {
	public static class TransformSnapshotUtil {
		public static TransformSnapshot New(Vector3 eyePosition, Quaternion eyeRotation, float eyeDistance) {
			Vector3 position = VectorR.MoveStraight(
				position: eyePosition,
				rotation: eyeRotation,
				distance: eyeDistance,
				forward: true);
			Quaternion rotation = eyeRotation;
			return new TransformSnapshot(position, rotation);
		}

		public static Vector3 EyePosition(this TransformSnapshot self, float eyeDistance) {
			return VectorR.MoveStraight(
				position: self.Position,
				rotation: self.Rotation,
				distance: eyeDistance,
				forward: false);
		}

		public static void ConstrainToEye(this TransformSnapshot self, Transform transform, float eyeDistance) {
			transform.position = New(
				eyePosition: self.EyePosition(eyeDistance),
				eyeRotation: transform.rotation,
				eyeDistance: eyeDistance).Position;
		}

		public static void Smooth(this TransformSnapshot self, Transform transform, float amount, bool translate = true, bool rotate = true) {
			if (translate) transform.SmoothTranslate(self.Position, amount);
			if (rotate) transform.SmoothRotate(self.Rotation, amount);
		}
	}
}
