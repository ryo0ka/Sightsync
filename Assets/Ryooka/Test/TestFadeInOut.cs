using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Ryooka.Scripts.Test {
    public class TestFadeInOut: MonoBehaviour {
        public enum OP { OR, AND, SC }

        public Renderer renderer1;
        public Renderer renderer2;
        public int tick;
        public bool dir;
        public OP op;

        void Start() {
            var t = (dir) ? renderer2 : renderer1;
            t.SetAlpha(0);
        }

        IEnumerator Fade() {
            dir = !dir;
            var m = (dir) ? renderer1 : renderer2;
            var n = (dir) ? renderer2 : renderer1;
            var me = m.Fade(0, 1, tick);
            var ne = n.Fade(1, 0, tick + 20);
            switch (op) {
                case OP.AND:
                    while (me.MoveNext() && ne.MoveNext()) yield return null;
                    break;
                case OP.OR:
                    while (me.MoveNext() || ne.MoveNext()) yield return null;
                    break;
                case OP.SC:
                    while (me.MoveNext() | ne.MoveNext()) yield return null;
                    break;
            }
        }

        public void FadeNow() {
            StartCoroutine(Fade());
        }

        void OnGUI() {
            if (GUILayout.Button("Fade")) {
                FadeNow();
            }
            if (GUILayout.Button("&&")) {
                op = OP.AND;
            }
            if (GUILayout.Button("||")) {
                op = OP.OR;
            }
            if (GUILayout.Button("|")) {
                op = OP.SC;
            }
        }
    }
}
