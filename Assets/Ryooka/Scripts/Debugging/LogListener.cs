using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.Events;

namespace Assets.Ryooka.Scripts.Debugging {
    [DisallowMultipleComponent]
    public class LogListener: MonoBehaviour {
		[Serializable]
		public class StringEvent: UnityEvent<string> { }

		public List<string> exclude;
		public StringEvent onLogEnter;

        private void OnEnable() {
            Application.logMessageReceived += ReceiveLog;
        }

        private void OnDisable() {
            Application.logMessageReceived -= ReceiveLog;
        }

        private void ReceiveLog(string log, string trace, LogType type) {
			if (exclude.Count == 0 || !exclude.Any(p => Regex.IsMatch(log, p))) {
				onLogEnter.Invoke(log);
			}
        }

        public void Log(string v) {
            Debug.Log(v);
        }

        public void Log(Vector2 v) {
            Debug.Log(v);
        }

        public void Log(float v) {
            Debug.Log(v);
        }

        public void Log() {
            Debug.Log("<>");
        }
    }
}