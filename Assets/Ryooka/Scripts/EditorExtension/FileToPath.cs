using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Ryooka.Scripts.EditorExtension {
	public class FileToPath : PropertyAttribute {
		public enum Root { ROOT, STREAMING_ASSETS, }

		static string PathOf(Root root) {
			switch (root) {
				case Root.STREAMING_ASSETS: return "Assets/StreamingAssets";
				default: return "";
			}
		}

		Root root;

		public FileToPath(Root root) {
			this.root = root;
		}

		public FileToPath() : this(Root.ROOT) { }

		public string RootPath() {
			return PathOf(root);
		}
	}
}
