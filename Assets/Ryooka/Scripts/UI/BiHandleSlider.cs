using Assets.Ryooka.Scripts.Extension;
using Assets.Ryooka.Scripts.General;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Ryooka.Scripts.UI {

    /*
    3 unit systems are used in this script (just as in `UnityEngine.UI.Slider`):
    Local position      : Position from the anchor of this rect. `posL` etc.
    Normalized position : 0 to 1 between min/max bounds. `posN` etc.
    Value               : User-defined value that's translated from a normalized position.
    Only values should be visible outside the script so as to avoid confusion.
    Normalized positions should be the main unit of internal communication.

    A handler and body must be a direct child of this object to appropriately function.
    Current values are only accessible through registered callback events.
    */
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class BiHandleSlider : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
		[Serializable]
		public class FloatEvent : UnityEvent<float> { }

        new RectTransform transform { //Overrides `transform` from `Transform` to `RectTransform`, for convenience.
            get { return base.transform as RectTransform; }
        }

        public RectTransform leftHandle; //Left handle of the slider.
        public RectTransform rightHandle; //Right handle of the slider.
        public RectTransform body; //Background that underlies between those handles.

        public float padding; //Minimum distance between two handles.

        public bool lockLeftHandle; //True to lock the left handle at a position.
        public bool lockRightHandle; //True to lock the right handle at a position.

        public float minValue;
        public float maxValue;

        //These events are private because their `Invoke` shouldn't be manually called.
        //These events are however displayed in the inspector as being `[SerializeField]`.
        //Use another set of events defined bellow to register runtime events manually.
        [SerializeField] FloatEvent _onLeftValueChanged; //Called when the left handle moves.
        [SerializeField] FloatEvent _onRightValueChanged; //Called when the right handle moves.
        [SerializeField] FloatEvent _onValueDiffChanged; //Called when the gap between left/right values changes.

        public event Action<float> onLeftValueChanged;
        public event Action<float> onRightValueChanged;
        public event Action<float> onValueDiffChanged;

        enum Handle { NONE, MIN, MAX }
        Handle dragging; //Currently operated handle.

        Handle lastDraggedHandle; //Previously operated handle.
        float lastDiff; //Previous distance between left/right handles. Normalized.

        void Reset() {
            padding = 10f; //A temporary value; if 0, two handles can overwrap each other.
            minValue = 0;
            maxValue = 1;
            dragging = lastDraggedHandle = Handle.NONE;
        }

        void Start() {
            onLeftValueChanged = _ => { };
            onRightValueChanged = _ => { };
            onValueDiffChanged = _ => { };

            //Executes with the initial positions of left/right handles.
            OnLeftPosNChanged(GetNormalizedPositionOf(leftHandle));
            OnRightPosNChanged(GetNormalizedPositionOf(rightHandle));
        }

        //Executed when any components under this rect start to be dragged.
        void IBeginDragHandler.OnBeginDrag(PointerEventData pointer) {
            if (leftHandle.Contains(pointer, PointerType.PRESS) && !lockLeftHandle) {
                dragging = lastDraggedHandle = Handle.MIN;
            }
            if (rightHandle.Contains(pointer, PointerType.PRESS) && !lockRightHandle) {
                dragging = lastDraggedHandle = Handle.MAX;
            }
        }

        //Executed while any components under this rect are being dragged.
        void IDragHandler.OnDrag(PointerEventData pointer) {
            Vector2 pointerPosL; //local position of pointer in this slider's rect.
            var inside = transform.LocalPosition(pointer, out pointerPosL, PointerType.PRESS);
            var pointerPosN = NormalizeFromLocalPosition(pointerPosL.x);
            if (inside && dragging == Handle.MIN) UpdateLeftHandlePosN(pointerPosN);
            if (inside && dragging == Handle.MAX) UpdateRightHandlePosN(pointerPosN);
        }

        //Executed when the dragging ends.
        void IEndDragHandler.OnEndDrag(PointerEventData pointer) {
            dragging = Handle.NONE;
        }

        void OnLeftPosNChanged(float posN) {
            body.SetAnchorMinX(posN); //Body's left bound follows the left handle.
            var val = DenormalizeToValue(posN);
            _onLeftValueChanged.Invoke(val);
            onLeftValueChanged(val);
            ValidateDiffChanged(Handle.MIN);
        }

        void OnRightPosNChanged(float posN) {
            body.SetAnchorMaxX(posN); //Body's right bound follows the right handle.
            var val = DenormalizeToValue(posN);
            _onRightValueChanged.Invoke(val);
            onRightValueChanged(val);
            ValidateDiffChanged(Handle.MAX);
        }

        float DenormalizeToValue(float posN) {
            return MathR.Map(posN, 0, 1, minValue, maxValue);
        }

        float NormalizeFromValue(float val) {
            return MathR.Map(val, minValue, maxValue, 0, 1);
        }

        float DenormalizeToLocalPosition(float posN) {
            return MathR.Map(posN, 0, 1, MinLocalPosition(), MaxLocalPosition());
        }

        float NormalizeFromLocalPosition(float posL) {
            return MathR.Map(posL, MinLocalPosition(), MaxLocalPosition(), 0, 1);
        }

        float MinLocalPosition() { //Not a handle's position.
            return transform.MinLocalPosition().x;
        }

        float MaxLocalPosition() { //Not a handle's position.
            return transform.MaxLocalPosition().x;
        }

        //Notifies if the gap between left/right handles has changed since the last call.
        void ValidateDiffChanged(Handle updatedHandle) {
            var diff = GetRightHandleNormalizedPosition() 
                     - GetLeftHandleNormalizedPosition();
            if (diff != lastDiff) {
                lastDiff = diff;
                var diffV = DenormalizeToValue(diff);
                _onValueDiffChanged.Invoke(diffV);
                onValueDiffChanged(diffV);
            }
        }

        void UpdateLeftHandlePosN(float desiredPosN) {
            var minPosL = transform.rect.min.x; //Left handle's left bound is the slider's left bound.
            var maxPosL = rightHandle.localPosition.x - padding; //Left handle's right bound is the right handle.
            UpdateHandlePos(
                handle: leftHandle,
                newPosN: desiredPosN,
                minPosN: NormalizeFromLocalPosition(minPosL),
                maxPosN: NormalizeFromLocalPosition(maxPosL),
                onPosNChanged: OnLeftPosNChanged);
        }

        void UpdateRightHandlePosN(float desiredPosN) {
            var minPosL = leftHandle.localPosition.x + padding; //Right handle's left bound is the left handle.
            var maxPosL = transform.rect.max.x; //Right handle's right bound is the slider's right bound.
            UpdateHandlePos(
                handle: rightHandle,
                newPosN: desiredPosN,
                minPosN: NormalizeFromLocalPosition(minPosL),
                maxPosN: NormalizeFromLocalPosition(maxPosL),
                onPosNChanged: OnRightPosNChanged);
        }

        //Updating left/right handles is essentially the same process with different parameters.
        void UpdateHandlePos(
            RectTransform handle, 
            float newPosN, 
            float minPosN, 
            float maxPosN, 
            Action<float> onPosNChanged)
        {
            newPosN = Mathf.Clamp(newPosN, minPosN, maxPosN); //Fits the new position in between the left/right bounds.
            var oldPosN = GetNormalizedPositionOf(handle);
            if (newPosN != oldPosN) { //If the position hasn't changed, don't invoke callbacks.
                var newPosL = DenormalizeToLocalPosition(newPosN);
                handle.SetLocalPositionX(newPosL); //Applies the new position to the handle.
                onPosNChanged(newPosN);
            }
        }

        float GetNormalizedPositionOf(RectTransform t) {
            //Normalizes the position (0-1) between the slider's left/right bounds.
            return transform.Normalize(t.localPosition).x;
        }

        float GetRightHandleNormalizedPosition() { //Normalized position (0-1).
            return GetNormalizedPositionOf(rightHandle);
        }

        float GetLeftHandleNormalizedPosition() { //Normalized position (0-1).
            return GetNormalizedPositionOf(leftHandle);
        }
        
        public void SetLength(float length) {
            var lengthN = NormalizeFromValue(length);
            SetNormalizedLength(lengthN);
        }

        //Achieves the given distance between left/right handles, moving the lastly controlled handle.
        //TODO: The result may be inaccurate if no handle is specified or no handles are locked.
        void SetNormalizedLength(float length) {
            var handle = Handle.NONE;
            if (lockLeftHandle && lockRightHandle) return; //Can't perform it with both handles locked.
            else if (lockLeftHandle)  handle = Handle.MAX;
            else if (lockRightHandle) handle = Handle.MIN;
            else                      handle = lastDraggedHandle; //Selects the last modified handle if all unlocked.
            SetNormalizedLength(length, handle);
        }

        //Achieves the given distance between left/right handles, moving the specified handle.
        //TODO: The result may be inaccurate if no handle is specified or no handles are locked.
        void SetNormalizedLength(float lengthN, Handle handle) {
            var leftPosN = GetLeftHandleNormalizedPosition();
            var rightPosN = GetRightHandleNormalizedPosition();
            switch (handle) {
                case Handle.NONE:
                    //TODO This is not even trying to get it right.
                    SetNormalizedLength(lengthN / 2, Handle.MIN);
                    SetNormalizedLength(lengthN / 2, Handle.MAX);
                    break;
                case Handle.MIN:
                    var newLeftPosN = Mathf.Clamp01(rightPosN - lengthN);
                    UpdateLeftHandlePosN(newLeftPosN);
                    break;
                case Handle.MAX:
                    var newRightPosN = Mathf.Clamp01(leftPosN + lengthN);
                    UpdateRightHandlePosN(newRightPosN);
                    break;
            }
        }
    }
}
