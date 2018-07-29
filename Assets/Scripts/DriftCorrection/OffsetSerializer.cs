using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Assets.Ryooka.Scripts.General;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Scripts.DriftCorrection {
	public class OffsetSerializer : MonoBehaviour {
		[SerializeField]
		public class TransformData {
			public Vector3 position;
			public float rotation;
		}

		[SerializeField]
		Transform world;

		[SerializeField]
		Transform player;

		[SerializeField]
		InputField fileNameInputField;

		[SerializeField]
		Button loadButton;

		[SerializeField]
		Button saveButton;
		
		void Start() {
			loadButton.onClick.AddListener(Load);
			saveButton.onClick.AddListener(Save);
		}

		void Load() {
			var data = JsonUtility.FromJson<TransformData>(File.ReadAllLines(DataFilePath())[0]);
			world.localPosition = data.position;
			world.SetLocalEulerAngles(y: data.rotation);
			Debug.Log("Loaded offset from disk.");
		}

		void Save() {
			var data = new TransformData {
				position = world.localPosition,
				rotation = world.localEulerAngles.y,
			};
			File.WriteAllLines(DataFilePath(), new string[] { JsonUtility.ToJson(data) });
			Debug.Log("Saved offset to disk.");
		}

		string DataFilePath() {
			return IOUtil.PersistentDataFilePath(fileNameInputField.text, "ssos");
		}
	}
}
