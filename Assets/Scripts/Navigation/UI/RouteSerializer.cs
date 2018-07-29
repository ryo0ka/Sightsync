using Assets.Ryooka.Scripts.General;
using System;
using System.IO;
using UnityEngine;
using System.Linq;
using Assets.Ryooka.Scripts.Debugging;

namespace Assets.Scripts.Navigation.UI {
	// Route's serializable data, used to save/load routes to/from the disk
	[Serializable]
	public class RouteData {
		public string name;

		public Vector3[] points;

		public float maxDistance;
	}

	// All the existing routes being serialied for loading and saving.
	[Serializable]
	public class RouteDataSet {
		public RouteData[] routes;

		public float offsetSpeed;

		public float intervalSeconds;
	}

	public class RouteSerializer : Singleton<RouteSerializer> {
		const string EXT = "ssrc";

		public RouteDataSet Load(string fileName) {
			string path = DataFilePath(fileName);
			string line = File.ReadAllLines(path)[0];

			RouteDataSet routes = JsonUtility.FromJson<RouteDataSet>(line);

			Debug.Log("loaded Routes from " + path + " with " + routes.routes.Length + " paths.");

			return routes;
		}

		public void Save(RouteDataSet routes, string fileName) {
			Debug.Log(routes.routes.Select(n => n.name).ToArray().Show());

			string line = JsonUtility.ToJson(routes);

			string path = DataFilePath(fileName);

			File.WriteAllLines(path, new string[] { line });

			Debug.Log("saving Routes to " + path + " with " + routes.routes.Length + " paths.");
		}

		string DataFilePath(string fileName) {
			return IOUtil.PersistentDataFilePath(fileName, EXT);
		}
	}
}
