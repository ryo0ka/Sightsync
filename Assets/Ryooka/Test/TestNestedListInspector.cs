using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Ryooka.Test {
	public class TestNestedListInspector : MonoBehaviour {
		[Serializable]
		public class HasList {
			public List<int> ints;
		}

		public List<HasList> lists;
	}
}
