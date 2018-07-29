using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.Extension {
    public class TransformExt : MonoBehaviour {
        public void SetLocalRotationX(float x) {
            transform.ModifyLocalRotation(rot => rot.WithX(x));
        }

        public void SetLocalEulerAngleX(float x) {
            transform.ModifyLocalEulerAngles(r => r.WithX(x));
		}

		public void SetLocalEulerAngleZ(float z) {
			transform.ModifyLocalEulerAngles(r => r.WithZ(z));
		}

		public void SetLocalEulerAngleY(float y) {
			transform.ModifyLocalEulerAngles(r => r.WithY(y));
		}

		public void SetEulerAngleY(float y) {
			transform.ModifyEulerAngles(r => r.WithY(y));
		}

		public void AddLocalEulerAngles(Vector2 added) {
			transform.ModifyLocalEulerAngles(
				o => o.WithX(o.x + added.x).WithY(o.y + added.y));
		}

		public void SetRotationXZ(Vector2 rotation) {
			transform.localEulerAngles = rotation.WithY(0);
		}

		public void AddLocalTranslationX(float x) {
			transform.Translate(new Vector3(x, 0, 0));
		}

		public void AddLocalTranslationZ(float z) {
			transform.Translate(new Vector3(0, 0, z));
		}
	}
}
