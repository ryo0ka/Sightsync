using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Ryooka.Scripts.General;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Ryooka.Scripts.Extension {
	//`PointerEventData` handles two cases of pointer input: "enter" and "press".
	//Most times "press" is what we're interested in, but we support "enter" too just in case.
	public enum PointerType { PRESS, ENTER, }

	//Provides helper extension methods for `Transform` and `RectTransform`.
	public static class TransformUtil {
		//True if the given pointer is inside of this rect.
		public static bool Contains(this RectTransform self, PointerEventData pointer, PointerType eventType) {
			Vector2 pos = PointerPosition(pointer, eventType);
			Camera camera = PointerCamera(pointer, eventType);
			return RectTransformUtility.RectangleContainsScreenPoint(self, pos, camera);
		}

		//Retrieves the pointer's local position in this rect.
		//If the pointer is inside, returns true and stores the location value into the ref.
		//To get the global position, use `Transform::TransformPoint()` to the returned value.
		public static bool LocalPosition(this RectTransform self, PointerEventData pointer, out Vector2 position, PointerType eventType) {
			Vector2 pos = PointerPosition(pointer, eventType);
			Camera camera = PointerCamera(pointer, eventType);
			return RectTransformUtility.ScreenPointToLocalPointInRectangle(self, pos, camera, out position);
		}

		static Vector2 PointerPosition(PointerEventData pointer, PointerType type) {
			return (type == PointerType.PRESS)
				? pointer.pressPosition
				: pointer.position;
		}

		static Camera PointerCamera(PointerEventData pointer, PointerType type) {
			return (type == PointerType.PRESS)
				? pointer.pressEventCamera
				: pointer.enterEventCamera;
		}

		//Retrieves the position of the min bound (top left) of this rect.
		public static Vector2 MinLocalPosition(this RectTransform self) {
			var w = self.rect.width;
			var h = self.rect.height;
			var c = self.pivot;
			return new Vector2(w * -c.x, h * -c.y);
		}

		//Retrieves the position of the max bound (bottom right) of this rect.
		public static Vector2 MaxLocalPosition(this RectTransform self) {
			var w = self.rect.width;
			var h = self.rect.height;
			var c = self.pivot;
			return new Vector2(w * (1 - c.x), h * (1 - c.y));
		}

		//Clamps the given local position so it fits inside this rect's bounds.
		public static Vector2 Clamp(this RectTransform self, Vector2 pos) {
			var min = self.MinLocalPosition();
			var max = self.MaxLocalPosition();
			return pos.Clamp(min, max);
		}

		//Normalizes the given local position so the min is (0, 0) and max is (1, 1).
		//It may go out of 0-1 if the position is out of this rect's bounds.
		public static Vector2 Normalize(this RectTransform self, Vector2 pos) {
			var min = self.MinLocalPosition();
			var max = self.MaxLocalPosition();
			return pos.Normalize(min, max);
		}

		//Translates a normalized position to a local position.
		public static Vector2 Denormalize(this RectTransform self, Vector2 normalizedPos) {
			var min = self.MinLocalPosition();
			var max = self.MaxLocalPosition();
			return normalizedPos.Map(Vector2.zero, Vector2.one, min, max);
		}

		//Normalizes the given rect's local position based on the given parent rect.
		public static Vector2 NormalizedLocalPosition(this RectTransform self, RectTransform parent) {
			return parent.Normalize(self.localPosition);
		}

		//Normalizes the given rect's local position based on its parent rect.
		public static Vector2 NormalizedLocalPosition(this RectTransform self) {
			return self.NormalizedLocalPosition(self.parent as RectTransform);
		}

		//Denormalizes the given normal position based on the given parent rect and sets it in the given rect's local position.
		public static void SetNormalizedLocalPosition(this RectTransform self, Vector2 normalizedLocalPosition, RectTransform parent) {
			var localPosition = parent.Denormalize(normalizedLocalPosition);
			self.localPosition = localPosition;
		}

		//Denormalizes the given normal position based on the given rect' parent and sets it in the given rect's local position.
		public static void SetNormalizedLocalPosition(this RectTransform self, Vector2 normalizedLocalPosition) {
			self.SetNormalizedLocalPosition(normalizedLocalPosition, self.parent as RectTransform);
		}

		//Seeking for some general method to get things easier.
		public static void ModifyAnchorMin(this RectTransform self, Func<Vector2, Vector2> mod) {
			self.anchorMin = mod(self.anchorMin);
		}

		public static void ModifyAnchorMax(this RectTransform self, Func<Vector2, Vector2> mod) {
			self.anchorMax = mod(self.anchorMax);
		}

		public static void SetAnchorMinX(this RectTransform self, float x) {
			self.ModifyAnchorMin(old => old.WithX(x));
		}

		public static void SetAnchorMaxX(this RectTransform self, float x) {
			self.ModifyAnchorMax(old => old.WithX(x));
		}

		public static void ModifyLocalPosition(this RectTransform self, Func<Vector2, Vector2> mod) {
			self.localPosition = mod(self.localPosition);
		}

		public static void SetLocalPositionX(this RectTransform self, float x) {
			self.ModifyLocalPosition(o => o.WithX(x));
		}

		public static Vector3 Size(this Transform self) {
			if (self is RectTransform)
				return (self as RectTransform).Size().To3(0);

			var meshSize = self.GetComponent<MeshFilter>().sharedMesh.bounds.size;
			return VectorR.Zip(meshSize, self.lossyScale, (m, n) => m * n);
		}

		public static Vector2 Size(this RectTransform self) {
			return self.rect.size;
		}

		public static void ModifyLocalRotation(this Transform self, Func<Quaternion, Quaternion> f) {
			self.localRotation = f(self.localRotation);
		}

		public static void ModifyLocalEulerAngles(this Transform self, Func<Vector3, Vector3> f) {
			self.localEulerAngles = f(self.localEulerAngles);
		}

		public static void ModifyEulerAngles(this Transform self, Func<Vector3, Vector3> f) {
			self.eulerAngles = f(self.eulerAngles);
		}

		public static Vector3 AngleDifferenceWith(this Transform self, Transform that) {
			return self.eulerAngles.DifferenceWith(that.eulerAngles);
		}

		public static void SmoothTranslate(this Transform self, Vector3 previous, float amount) {
			self.position = self.position.Smooth(previous, amount);//.Map(MathR.ZeroIllegal);
		}

		public static void SmoothRotate(this Transform self, Quaternion previous, float amount) {
			self.rotation = self.rotation.Smooth(previous, amount);//.Map(MathR.ZeroIllegal);
		}

		public static void SetRotationAround(this Transform self, Vector3 position, Quaternion rotation) {
			float distance = Vector3.Distance(position, self.position);
			self.rotation = rotation;
			Vector3 fix = Math3d.SetVectorLength(self.forward, distance);
			Vector3 prefocus = self.position + fix;
			self.position += position - prefocus;
		}

		public static void SetPosition(this Transform self, float? x = null, float? y = null, float? z = null) {
			Vector3 position = self.position;
			float _x = x.GetValueOrDefault(position.x);
			float _y = y.GetValueOrDefault(position.y);
			float _z = z.GetValueOrDefault(position.z);
			self.position = new Vector3(_x, _y, _z);
		}

		public static void SetLocalPosition(this Transform self, float? x = null, float? y = null, float? z = null) {
			Vector3 position = self.localPosition;
			float _x = x.GetValueOrDefault(position.x);
			float _y = y.GetValueOrDefault(position.y);
			float _z = z.GetValueOrDefault(position.z);
			self.localPosition = new Vector3(_x, _y, _z);
		}

		public static void SetX(this Transform self, float x) {
			self.SetPosition(x: x);
		}

		public static void SetY(this Transform self, float y) {
			self.SetPosition(y: y);
		}

		public static void SetZ(this Transform self, float z) {
			self.SetPosition(z: z);
		}

		public static void SetEulerAngles(this Transform self, float? x = null, float? y = null, float? z = null) {
			Vector3 angles = self.eulerAngles;
			float _x = x.GetValueOrDefault(angles.x);
			float _y = y.GetValueOrDefault(angles.y);
			float _z = z.GetValueOrDefault(angles.z);
			self.eulerAngles = new Vector3(_x, _y, _z);
		}

		public static void SetLocalEulerAngles(this Transform self, float? x = null, float? y = null, float? z = null) {
			Vector3 angles = self.localEulerAngles;
			float _x = x.GetValueOrDefault(angles.x);
			float _y = y.GetValueOrDefault(angles.y);
			float _z = z.GetValueOrDefault(angles.z);
			self.localEulerAngles = new Vector3(_x, _y, _z);
		}

		public static Vector3 GetPositionIn(this Transform self, Space space) {
			switch (space) {
				case Space.Self:
					return self.localPosition;
				case Space.World:
					return self.position;
				default:
					return Vector3.zero;
			}
		}

		public static void SetPositionSpaced(this Transform self, Space space, Vector3 value) {
			switch (space) {
				case Space.Self:
					self.localPosition = value;
					break;
				case Space.World:
					self.position = value;
					break;
				default:
					break;
			}
		}

		public static Quaternion GetRotationIn(this Transform self, Space space) {
			switch (space) {
				case Space.Self:
					return self.localRotation;
				case Space.World:
					return self.rotation;
				default:
					return Quaternion.identity;
			}
		}

		public static void SetRotationSpaced(this Transform self, Space space, Quaternion value) {
			switch (space) {
				case Space.Self:
					self.localRotation = value;
					break;
				case Space.World:
					self.rotation = value;
					break;
				default:
					break;
			}
		}

		public static bool HasParent(this Transform self) {
			return self.parent != null;
		}

		public static bool IsRoot(this Transform self) {
			return !self.HasParent();
		}

		public static IEnumerable<Transform> GetTransformsInFirstChildren(this Transform self) {
			foreach (Transform child in self) {
				yield return child;
			}
		}

		// depth-first search.
		// can specify the depth of search (-1 to go the deepest).
		// can specify whether including the root's components or not.
		public static IEnumerable<T> GetComponents<T>(this Transform self, int depth, bool includeSelf) where T : Component {
			if (depth == 0) {
				yield break;
			} else {
				if (includeSelf) {
					foreach (T selfComponent in self.GetComponents<T>()) {
						yield return selfComponent;
					}
				}

				foreach (Transform child in self.GetTransformsInFirstChildren()) {
					foreach (T childComponent in child.GetComponents<T>()) {
						yield return childComponent;
					}

					foreach (var gchild in child.GetComponents<T>(depth - 1, false)) {
						yield return gchild;
					}
				}
			}
		}

		public static IEnumerable<Transform> Parents(this Transform self, bool includeSelf = false) {
			for (int i = 0; self != null; i++) {
				if (!includeSelf && i == 0) {
					//pass
				} else {
					yield return self;
				}
				self = self.parent;
			}
		}

		// Measures the scale of this transform from the specified parent object.
		// If the parent object is not found up to the root object, throws an exception.
		public static Vector3 LocalScaleFrom(this Transform self, Transform parent) {
			Vector3 scale = Vector3.one;
			Transform current = self;
			bool parentFound = false;
			bool reachedRoot = false;

			while (!parentFound && !reachedRoot) {
				scale = new Vector3(
					scale.x * current.localScale.x,
					scale.y * current.localScale.y,
					scale.z * current.localScale.z);

				if (current.parent == null) {
					reachedRoot = true;
				} else if (current.parent == parent) {
					parentFound = true;
				} else {
					current = current.parent;
				}
			}

			if (reachedRoot && !parentFound) {
				throw new Exception("Specified parent " + parent.name + " not found from " + self.name + ".");
            } else {
				return scale;
			}
		}

		public static void CloneValues(this Transform self, Transform original, Transform sharedParent) {
			self.SetParent(sharedParent);
			self.position = original.position;
			self.rotation = original.rotation;
			self.localScale = original.LocalScaleFrom(sharedParent.parent);
		}
	}
}
