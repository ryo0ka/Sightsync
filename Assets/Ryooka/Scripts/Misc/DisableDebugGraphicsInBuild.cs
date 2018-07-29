using UnityEngine;
using Assets.Ryooka.Scripts.EditorExtension;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Ryooka.Scripts.Misc {
	public class DisableDebugGraphicsInBuild : MonoBehaviour {
		public UnityLayer[] debugLayers;
		public bool disableInEditor;

		void Start() {
			if (disableInEditor	|| !Application.isEditor) {
				foreach (GameObject root in GameObjectUtil.GetSceneRootObjects()) {
					foreach	(Renderer renderer in root.GetComponents<Renderer>(-1, true)) {
						int layer = renderer.gameObject.layer;
						bool found = false;
						foreach (var debugLayer in debugLayers) {
							if (debugLayer.LayerIndex == layer) {
								found = true;
								break;
							}
						}
						if (found) {
							renderer.enabled = false;				
						}
					}
				}
			}
		}
	}
}