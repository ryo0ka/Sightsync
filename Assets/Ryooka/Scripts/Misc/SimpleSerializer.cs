using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.Misc {
	public static class SimpleSerializer {
		public static void Save<T>(string fileName, IEnumerable<T> values) {
			string path = FilePath(fileName);
			File.WriteAllLines(path, Serialize(values));
		}

		public static IEnumerable<T> Load<T>(string fileName) {
			string path = FilePath(fileName);
			return Deserialize<T>(File.ReadAllLines(path));
		}

		public static string[] Serialize<T>(IEnumerable<T> values) {
			List<string> lines = new List<string>();
			foreach (var target in values) {
				var line = JsonUtility.ToJson(target);
				lines.Add(line);
			}
			return lines.ToArray();
		}

		public static IEnumerable<T> Deserialize<T>(string[] lines) {
			return lines.Select(JsonUtility.FromJson<T>);
		}

		public static string FilePath(string fileName) {
			return IOUtil.PersistentDataFilePath(fileName + ".txt");
		}
	}
}
