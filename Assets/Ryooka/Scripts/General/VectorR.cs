using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Ryooka.Scripts.General {
	public static class VectorR {
		public enum Axis { X, Y, Z }

		static public Vector2 Map(this Vector2 valA, Vector2 minA, Vector2 maxA, Vector2 minB, Vector2 maxB) {
			var v = Vector2.zero;
			v.x = MathR.Map(valA.x, minA.x, maxA.x, minB.x, maxB.x);
			v.y = MathR.Map(valA.y, minA.y, maxA.y, minB.y, maxB.y);
			return v;
		}

		static public Vector2 Normalize(this Vector2 self, Vector2 min, Vector2 max) {
			self.x = MathR.Map(self.x, min.x, max.x, 0, 1);
			self.y = MathR.Map(self.y, min.y, max.y, 0, 1);
			return self;
		}

		static public Vector2 Denormalize(this Vector2 self, Vector2 min, Vector2 max) {
			return self.Map(Vector2.zero, Vector2.one, min, max);
		}

		static public Vector2 Clamp(this Vector2 self, Vector2 min, Vector2 max) {
			self.x = Mathf.Clamp(self.x, min.x, max.x);
			self.y = Mathf.Clamp(self.y, min.y, max.y);
			return self;
		}

		static public Vector3 Zip(Vector3 one, Vector3 two, Func<float, float, float> f) {
			return new Vector3(f(one.x, two.x), f(one.y, two.y), f(one.z, two.z));
		}

		static public Vector3 ZipWith(this Vector3 self, Vector3 that, Func<float, float, float> f) {
			return Zip(self, that, f);
		}

		static public Vector3 Map(this Vector3 self, Func<float, float> f) {
			return new Vector3(f(self.x), f(self.y), f(self.z));
		}

		static public bool All(this Vector3 self, Func<float, bool> f) {
			return self.AsEnumerable().All(f);
		}

		static public Vector2 WithX(this Vector2 self, float x) {
			self.x = x;
			return self;
		}

		static public Vector2 WithY(this Vector2 self, float y) {
			self.y = y;
			return self;
		}

		static public Vector3 WithX(this Vector3 self, float x) {
			self.x = x;
			return self;
		}

		static public Vector3 WithY(this Vector3 self, float y) {
			self.y = y;
			return self;
		}

		static public Vector3 WithZ(this Vector3 self, float z) {
			self.z = z;
			return self;
		}

		static public Vector2 To2(this Vector3 self) {
			return new Vector2(self.x, self.y);
		}

		static public Vector2 To2(this float self) {
			return new Vector2(self, self);
		}

		static public Vector3 To3(this Vector2 self, float z = 0) {
			return new Vector3(self.x, self.y, z);
		}

		static public Vector3 To3(this float self) {
			return new Vector3(self, self, self);
		}

		static public Quaternion WithX(this Quaternion self, float x) {
			self.x = x;
			return self;
		}

		static public Vector2 WithX(this float y, float x) {
			return new Vector2(x, y);
		}

		static public Vector2 WithY(this float x, float y) {
			return new Vector2(x, y);
		}

		static public Vector3 WithDelta(this Vector3 self, float? x = null, float? y = null, float? z = null) {
			float _x = (x.HasValue) ? x.Value : 0;
			float _y = (y.HasValue) ? y.Value : 0;
			float _z = (z.HasValue) ? z.Value : 0;
			return new Vector3(self.x + _x, self.y + _y, self.z + _z);
		}

		static public Vector3 Pole(Axis axis, float value) {
			switch (axis) {
				case Axis.X: return Vector3.zero.WithX(value);
				case Axis.Y: return Vector3.zero.WithY(value);
				case Axis.Z: return Vector3.zero.WithZ(value);
				default: throw new ArgumentException();
			}
		}

		static public Vector3 WithAxis(this Vector3 self, Axis axis, float value) {
			switch (axis) {
				case Axis.X: return self.WithX(value);
				case Axis.Y: return self.WithY(value);
				case Axis.Z: return self.WithZ(value);
				default: throw new ArgumentException();
			}
		}

		static public IEnumerable<float> AsEnumerable(this Vector3 self) {
			yield return self.x;
			yield return self.y;
			yield return self.z;
		}

		static public Vector3 Average(IEnumerable<Vector3> vs) {
			int length = vs.Count();
			Vector3 sum = Vector3.zero;
			foreach (var v in vs) sum += v;
			return sum / length;
		}

		static public Vector3 DifferenceWith(this Vector3 self, Vector3 that) {
			return (self - that).Map(n => {
				float absn = Mathf.Abs(n) % 360;
				return Mathf.Min(absn, 360 - absn);
			});
		}

		static public Vector3 Smooth(this Vector3 self, Vector3 previous, float amount) {
			Vector3 pp = previous;
			Vector3 cp = self;
			Vector3 rp = Vector3.zero;
			rp.x = MathR.Smooth(cp.x, pp.x, amount);
			rp.y = MathR.Smooth(cp.y, pp.y, amount);
			rp.z = MathR.Smooth(cp.z, pp.z, amount);
			return rp;
		}

		public static Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
			if (point == pivot) {
				return point;
			} else {
				Vector3 dir = point - pivot; // get point direction relative to pivot
				dir = Quaternion.Euler(angles) * dir; // rotate it
				point = dir + pivot; // calculate rotated point
				return point; // return it
			}
		}

		//https://forum.unity3d.com/threads/quaternions-forward-direction.26921/
		public static Vector3 MoveStraight(Vector3 position, Quaternion rotation, float distance, bool forward) {
			Vector3 direction = Vector3.forward * ((forward) ? 1 : -1);
			Vector3 backVector = Math3d.SetVectorLength(rotation * direction, distance);
			return position + backVector;
		}
	}
}
