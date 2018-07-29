using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

namespace Assets.Ryooka.Scripts.Misc {
    public class Popup : MonoBehaviour {
        public int targetTick;
        public Transform targetObject;

        void Reset() {
            targetTick = 100;
        }

        void OnEnable() {
            StartCoroutine(DisplayPopup().GetEnumerator());
        }

        IEnumerable DisplayPopup() {
            var tick = targetTick;
            while (tick-- > 0) {
                var posOnScreen = Camera.main.WorldToScreenPoint(targetObject.position);
                transform.position = posOnScreen;
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}
