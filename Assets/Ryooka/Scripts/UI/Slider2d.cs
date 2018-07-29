using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Assets.Ryooka.Scripts.Extension;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.UI {
	[SelectionBase]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class Slider2d: Selectable, IBeginDragHandler, IEndDragHandler, IDragHandler {
		[Serializable]
		public class Vector2Event: UnityEvent<Vector2> { }

		// In controller mode, behavior changes as follows:
		// - Whenever value is not zero, invoke callbacks (even if not changed).
		// - When handle is released, handle is moved to zero position.
		// Make sure min-max range contains zero, otherwise never stops sending events.
		[SerializeField]
		public bool controllerMode;

		[SerializeField]
		RectTransform field;

		[SerializeField]
		RectTransform handle;

		[SerializeField]
		Vector2 minValue; //won't respond to modification in inspector

		[SerializeField]
		Vector2 maxValue; //won't respond to modification in inspector

		[SerializeField]
		Vector2 currentValue; //won't respond to modification in inspector

		[SerializeField]
		public Vector2Event onValueChanged;

		bool isDragging;
		PointerEventData pointer;

		void Update() {
			if (interactable && isDragging) {
				Vector2 lPosition;
				if (field.LocalPosition(pointer, out lPosition, PointerType.ENTER)) {
					lPosition = field.Clamp(lPosition);

					// React when pointer and handle are at different locations.
					// If controllerMode is on, react anytime (until dragging ends).
					if (controllerMode || lPosition != (Vector2)handle.localPosition) {
						// Move handle to pointer.
						handle.localPosition = lPosition;

						// Notify new input.
						var nValue = L2N(lPosition);
						InvokeChanged(nValue);
					}
				}
			}
		}
		
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
			isDragging = true;
			pointer = eventData;
		}

		void IDragHandler.OnDrag(PointerEventData eventData) {
			isDragging = true; // just in case.
			pointer = eventData;
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
			isDragging = false;
			pointer = eventData;

			if (controllerMode) {
				// Move handle to center.
				handle.localPosition = N2L(Vector3.one / 2f);
			}
		}

		void InvokeChanged(Vector2 nValue) {
			var uValue = N2U(nValue);
			currentValue = uValue;
			onValueChanged.Invoke(uValue);
		}

		public void SetHandleCenter(float uRange, Vector2 uValue) {
			minValue = new Vector2(
				x: uValue.x - uRange / 2,
				y: uValue.y - uRange / 2);
			maxValue = new Vector2(
				x: uValue.x + uRange / 2,
				y: uValue.y + uRange / 2);
			var nValue = U2N(uValue);

			var lPosition = N2L(nValue);
			handle.localPosition = lPosition;

			InvokeChanged(nValue);
		}

		Vector2 U2N(Vector2 uValue) {
			return uValue.Normalize(minValue, maxValue);
		}

		Vector2 N2U(Vector2 nValue) {
			return nValue.Denormalize(minValue, maxValue);
		}

		Vector2 L2N(Vector2 lPosition) {
			return field.Normalize(lPosition);
		}

		Vector2 N2L(Vector2 nValue) {
			return field.Denormalize(nValue);
		}
	}
}
