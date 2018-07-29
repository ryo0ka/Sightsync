using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Ryooka.Scripts.Extension {
	public static class GameObjectUtil {
		public static T GetOrAddComponent<T>(this GameObject self) where T : Component {
			var found = self.GetComponent<T>();
			return (found == null) ? self.AddComponent<T>() : found;
		}

		public static IEnumerable<GameObject> GetGameObjectsInFirstChildren(this GameObject self) {
			return self.transform.GetTransformsInFirstChildren().Select(t => t.gameObject);
		}

		public static IEnumerable<T> GetComponents<T>(this GameObject self, int depth, bool includeSelf) where T : Component {
			return self.transform.GetComponents<T>(depth, includeSelf);
		}

		public static GameObject[] GetSceneRootObjects() {
			return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        }

#if UNITY_EDITOR
		public static bool IsPrefab(this GameObject self) {
			switch (PrefabUtility.GetPrefabType(self)) {
				case PrefabType.Prefab: return true;
				case PrefabType.ModelPrefab: return true;
				default: return false;
			}
		}

		public static bool IsPrefabOf(this GameObject self, GameObject sceneObject) {
			return PrefabUtility.GetPrefabParent(sceneObject) == self;
		}

		public static IEnumerable<GameObject> FindPrefabInstances(this GameObject self, GameObject sceneObject) {
			var prefab = PrefabUtility.FindPrefabRoot(sceneObject);

			if (prefab && prefab == self)
				yield return sceneObject;

			foreach (var child in sceneObject.GetGameObjectsInFirstChildren())
				foreach (var instance in self.FindPrefabInstances(child))
					yield return instance;
		}

		public static IEnumerable<GameObject> FindPrefabInstances(this GameObject self) {
			return GetSceneRootObjects().SelectMany(r => self.FindPrefabInstances(r));
		}
#endif
	}
}
