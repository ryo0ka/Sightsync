using Assets.Ryooka.Scripts.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Test {
	public class TestJsonDataFile : MonoBehaviour {

		[Serializable]
		public class Foo {
			public int bar;
			public string baz;
		}

		[Serializable]
		public class DataFileFoo : JsonDataFile<Foo> { }

		public DataFileFoo file;
		public Foo foo1;
		public Foo foo2;

		public bool read;

		void Start() {
			if (read) {
				Foo[] red = file.Read();
				foo1 = red[0];
				foo2 = red[1];
			} else {
				file.Write(new Foo[] { foo1, foo2 });
			}
		}
	}
}
