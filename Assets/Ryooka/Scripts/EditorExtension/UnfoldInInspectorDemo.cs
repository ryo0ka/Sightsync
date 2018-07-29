using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Ryooka.Scripts.EditorExtension {
	public class UnfoldInInspectorDemo : MonoBehaviour {
		[Serializable]
		public class CustomClass {
			public float floatValue;
			public List<string> listValue;
			public UnityEvent eventValue;
		}

		public int intValue;

		[UnfoldInInspector]
		public CustomClass customClassValue;

		public bool boolValue;
	}
}
