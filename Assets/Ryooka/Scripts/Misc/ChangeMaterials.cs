using UnityEngine;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Ryooka.Scripts.Misc {
	[ExecuteInEditMode]
	public class ChangeMaterials : MonoBehaviour {
		[SerializeField]
		bool executeInEditMode;

		[SerializeField]
		Material material;

		[SerializeField]
		GameObject target;

		void Reset() {
			executeInEditMode = false;
			target = gameObject;
		}

		void OnValidate() {
			if (!executeInEditMode && !Application.isPlaying) return;
			Change();
		}

		public void Change() {
			if (!material) return;
			foreach (var r in target.GetComponentsInChildren<Renderer>()) {
				r.material = material;
			} 
		}
	}
}
