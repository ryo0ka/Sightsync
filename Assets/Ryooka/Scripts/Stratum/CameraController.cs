using UnityEngine;

namespace Assets.Ryooka.Scripts.Stratum {
    public class CameraController: MonoBehaviour {
        public Transform focus;
        public float desiredSpeed;
        public float maxDepth;
        public float distance;

        Vector3 focalPoint {
            get { return focus.position; }
        }

        void Reset() {
            // Sets random default values.
            desiredSpeed = 10;
            maxDepth = 100;
            distance = 100;
        }

        bool IsUnderground() {
            var thisPos = transform.position.y;
            var thatPos = focalPoint.y;
            return thisPos < thatPos;
        }

        void KeepShapeWithFocalPoint() {
            KeepDistance(transform, focalPoint, distance);
            transform.LookAt(focalPoint);
        }

        void MoveVertical(bool up) {
            var p = up ? 1 : -1;
            var motion = new Vector3(0, desiredSpeed * p, 0);
            transform.Translate(motion, Space.Self);
            if (desiredSpeed <= 0) // Warns for invalid speed.
                Debug.LogWarningFormat("Suspecious speed: {0}", desiredSpeed);
        }

        public void MoveUp() {
            if (IsUnderground()) {
                MoveVertical(up: true);
            } else {
                #pragma warning disable 0162
                if (false) {
                    //TODO Lock if beyond focal point.
                } else {
                    MoveVertical(up: true);
                    KeepShapeWithFocalPoint();
                }
            }
        }

        public void MoveDown() {
            if (IsUnderground()) {
                var height = focalPoint.y - transform.position.y;
                if (height > maxDepth) {
                    // TODO Bounce?
                } else {
                    MoveVertical(up: false);
                }
            } else {
                MoveVertical(up: false);
                KeepShapeWithFocalPoint();
            }
        }

        static void KeepDistance(Transform transform, Vector3 target, float distance) {
            var dist = Vector3.Distance(transform.position, target);
            if (dist == distance) return;
            var direction = (target - transform.position).normalized;
            var vec = direction * (dist - distance);
            transform.localPosition += (vec - transform.forward);
        }
    }
}
