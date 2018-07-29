using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Ryooka.Scripts.Extension {
    public class MonoBehaviourEventTrigger : MonoBehaviour {
        public UnityEvent onAwake;
        public UnityEvent onStart;
        public UnityEvent onUpdate;
        public UnityEvent onEnable;
        public UnityEvent onDisable;

        void Awake() {
            onAwake.Invoke();
        }

        void Start() {
            onStart.Invoke();
        }

        void Update() {
            onUpdate.Invoke();
        }

        void OnEnable() {
            onEnable.Invoke();
        }

        void OnDisable() {
            onDisable.Invoke();
        }
    }
}
