using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Ryooka.Scripts.Extension {
	public static class UnityUtil {
		public static void Reload(this Scene self) {
			SceneManager.LoadScene(self.buildIndex);
		}

		public static void ReloadActiveScene() {
			SceneManager.GetActiveScene().Reload();
		}
	}
}
