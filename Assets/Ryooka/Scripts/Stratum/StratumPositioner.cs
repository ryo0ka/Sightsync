using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Stratum {
    public class StratumPositioner : MonoBehaviour {
        public List<Stratum> layers;
        public float distance;

        [SerializeField][HideInInspector]
        List<Stratum> layersRaised;

        void Reset() {
            layers = new List<Stratum>();
            layersRaised = new List<Stratum>();
        }

        void Update() {
            Migrate(layers, layersRaised, true);
            Migrate(layersRaised, layers, false);
        }

        void Migrate(List<Stratum> from, List<Stratum> to, bool up) {
            int start = -1;
            for (int i = 0; i < from.Count; ++i) {
                if (from[i].isUp == up) {
                    start = i;
                    break;
                }
            }
            if (start >= 0) {
                for (int i = from.Count - 1; i >= start; --i) {
                    var s = from[i];
                    from.RemoveAt(i);
                    to.Add(s);
                    s.isUp = up;
                    Move(s, up);
                }
            }
        }

        void Move(Stratum o, bool up) {
            var p = up ? 1 : -1;
            var t = new Vector3(0, distance * p, 0);
            o.gameObject.transform.Translate(t);
        }
    }
}
