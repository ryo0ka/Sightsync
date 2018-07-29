using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Stratum {
    public class MoveUpAndDown : MonoBehaviour {
        public float distance;
        public bool up;

        void Translate(bool up) {
            if (up == this.up) return;
            this.up = up;
            var p = up ? 1 : -1;
            var translation = new Vector3(0, distance * p, 0);
            transform.Translate(translation);
        }

        public void MoveUp() {
            Translate(true);
        }

        public void MoveDown() {
            Translate(false);
        }

        public void Toggle() {
            Translate(!up);
        }

        public void Move(bool up) {
            Translate(up);
        }
    }
}
