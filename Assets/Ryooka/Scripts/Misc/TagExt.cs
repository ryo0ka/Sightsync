using UnityEngine;
using System.Linq;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Ryooka.Scripts.Misc {
	[ExecuteInEditMode]
	public class TagExt : MonoBehaviour {
		public new string tag;

		[SerializeField]
		bool visible;

		void OnValidate() {
			SetVisible(visible);
		}

		public GameObject[] TaggedObjects() {
			return GameObject.FindGameObjectsWithTag(tag);
		}

		public void ToggleVisible() {
			SetVisible(!visible);
		}

		public void SetVisible(bool visible) {
			this.visible = visible;
			foreach (var r in TaggedObjects().SelectMany(o => o.GetComponents<Renderer>(-1, true))) {
				r.enabled = visible;
			}
		}
	}
}
