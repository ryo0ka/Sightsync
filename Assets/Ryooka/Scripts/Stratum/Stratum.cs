using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Stratum {
    public class Stratum: MonoBehaviour {
        public bool isUp;
        public bool IsUp {
            get { return isUp; }
            set { isUp = value; }
        }
    }
}
