using System;
using System.IO;
using UnityEngine;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.Misc {
	// Don't use this utility when speed/performance matters.
	// Make sure T is annotated Serializable and supported by JsonUtility.
	// See IOUtil::PresistentDataFilePath() for where files should be located.
	[Serializable]
	public class JsonDataFile<T> {
		[SerializeField]
		public string name;

		[SerializeField]
		public string extension;

		protected JsonDataFile() { }

		string[] Serialize(T[] values) {
			string[] lines = new string[values.Length];
			for (int i = 0; i < values.Length; i++) {
				lines[i] = JsonUtility.ToJson(values[i]);
			}
			return lines;
		}

		T[] Deserialize(string[] lines) {
			T[] values = new T[lines.Length];
			for (int i = 0; i < lines.Length; i++) {
				values[i] = JsonUtility.FromJson<T>(lines[i]);
			}
			return values;
		}

		string FilePath() {
			return IOUtil.PersistentDataFilePath(name, extension);
		}

		public T[] Read() {
			return Deserialize(File.ReadAllLines(FilePath()));
		}

		public void Write(T[] values) {
			File.WriteAllLines(FilePath(), Serialize(values));
		}

		public void Write(T value) {
			Write(new T[] { value });
		}
	}
}
