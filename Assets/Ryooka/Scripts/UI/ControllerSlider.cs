using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.UI {
	// Modified Slider, behaving as follows:
	// - Whenever value is not zero, invoke callbacks (even if not changed).
	// - When handle is released, move handle to zero.
	// Make sure min-max range contains zero, otherwise callbacks will never stop.
	public class ControllerSlider : Slider, IBeginDragHandler, IEndDragHandler {
		bool isDragging; // True while dragging.
		bool wasSet; // True if base class has updated slider during the last frame.
		float lastInput; // Last input to base class' Set().

		void Update() {
			if (interactable && isDragging) {
				if (!wasSet) {
					// Force invoke callbacks with the current value
					// while handle is not moving but being dragged (held).
					// base.Set() watches m_Value to check updates.
					m_Value = 0f;
					base.Set(lastInput, true);
				}
				wasSet = false;
			}
		}

		protected override void Set(float input, bool sendCallback) {
			wasSet = true;
			lastInput = input;
			base.Set(input, sendCallback);
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
			isDragging = true;
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
			isDragging = false;
			Set(0f, false);
		}

		// "Clicking" is not a use case.
		// When it happens, do invoke callbacks based on the point,
		// but immediately move handle to zero.
		public override void OnPointerDown(PointerEventData eventData) {
			base.OnPointerDown(eventData);
			Set(0f, false);
		}
	}
}
