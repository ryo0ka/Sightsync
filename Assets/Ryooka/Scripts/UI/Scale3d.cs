using Assets.Ryooka.Scripts.Extension;
using Assets.Ryooka.Scripts.General;
using UnityEngine;

namespace Assets.Ryooka.Scripts.UI {
    [ExecuteInEditMode]
    public class Scale3d : MonoBehaviour {
        public enum Axis { X, Y, Z }

		public Transform parent;
        public Transform target;
        public Axis parentAxis;
        public Axis targetAxis;

        float ParentSize() {
            return parent.transform.Size()[(int)parentAxis]; //TODO zero
        }

        float TargetSize() {
            return target.Size()[(int)targetAxis];
        }

		Vector3 CalculateLocalScale() {
			var lossyScale = target.transform.lossyScale;
			var scale = TargetSize() / ParentSize();
			return VectorR.Map(lossyScale, n => n / scale);
		}

        void LateUpdate() {
			if (parent == null || target == null) return;
			var last = target.transform.localScale;
			var current = CalculateLocalScale();
			if (last != current) target.transform.localScale = current;
        }
    }
}
