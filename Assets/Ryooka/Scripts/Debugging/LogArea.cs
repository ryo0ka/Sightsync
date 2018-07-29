using Assets.Ryooka.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Debugging {
	public class LogArea : TextQueue {
		[SerializeField]
		bool drawStackTrace;

		[HideInInspector]
		public bool activate;

		void Reset() {
			activate = true;
		}

		void OnEnable() {
			Application.logMessageReceived += ReceiveLog;
		}

		void OnDisable() {
			Application.logMessageReceived -= ReceiveLog;
		}

		void ReceiveLog(string log, string trace, LogType type) {
			if (!activate) return;

			string line = log;
			if (drawStackTrace && type != LogType.Log)
				line += " (" + trace + ")";
			AddLine(line);
		}
	}
}
