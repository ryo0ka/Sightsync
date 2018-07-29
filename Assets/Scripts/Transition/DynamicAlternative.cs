using System.Linq;
using UnityEngine;
using Assets.Ryooka.Scripts.Extension;
using System.Collections.Generic;

namespace Assets.Scripts.Transition {
	[RequireComponent(typeof(MeshRenderer))]
	public class DynamicAlternative : MonoBehaviour {
		class RendererPair {
			public MeshRenderer s; // static
			public MeshRenderer d; // dynamic
		}

		[SerializeField]
		bool showDynamic;

		List<RendererPair> renderers;

		void Awake() {
			renderers = new List<RendererPair>();
			Transform dParent = new GameObject(name + " (Dynamic)").transform;
			dParent.SetParent(transform.parent);

			foreach (MeshRenderer sRenderer in gameObject.GetComponents<MeshRenderer>(-1, true)) {
				Transform sObject = sRenderer.transform;
				Transform dObject = new GameObject(sRenderer.name + " (Dynamic)").transform;

				dObject.CloneValues(sObject, dParent);

				MeshFilter sFilter = sRenderer.GetComponent<MeshFilter>();
				dObject.gameObject.AddComponent<MeshFilter>().mesh = sFilter.sharedMesh;

				MeshRenderer dRenderer = dObject.gameObject.AddComponent<MeshRenderer>();
				dRenderer.materials = sRenderer.materials;
				dRenderer.CopyLightMapSettings(sRenderer);

				renderers.Add(new RendererPair {
					s = sRenderer,
					d = dRenderer,
				});
			}
		}

		void OnValidate() {
			if (renderers != null) {
				ShowDynamic(showDynamic);
			}
		}

		public void ShowDynamic(bool dynamic) {
			foreach (RendererPair r in renderers) {
				r.d.enabled = dynamic;
				r.s.enabled = !dynamic;
			}
		}
	}
}
