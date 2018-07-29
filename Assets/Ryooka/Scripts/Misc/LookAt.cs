using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Misc {
    public class LookAt : MonoBehaviour {
        public Transform target;

        void Update() {
            transform.LookAt(target);
        }
    }
}
