using System;
using System.IO;
using UnityEngine;

namespace Assets.Ryooka.Scripts.General {
	public static class IOUtil {
		public static string PersistentDataFolderPath() {
			switch (Application.platform) {
				case RuntimePlatform.Android:
					return Application.persistentDataPath;
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					return Application.streamingAssetsPath;
				default:
					throw new ArgumentException(
						message: "Unsupported platform",
						paramName: Application.platform.ToString());
			}
		}

		public static string PersistentDataFilePath(string name, string ext = null) {
			var dirPath = PersistentDataFolderPath();
			var _name = (ext == null) ? name : name + "." + ext;
			return Path.Combine(dirPath, _name);
		}
	}
}
