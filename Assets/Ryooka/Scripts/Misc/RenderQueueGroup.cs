using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Ryooka.Scripts.Extension;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.Misc {
	public class RenderQueueGroup : MonoBehaviour {
		public enum RenderQueueType {
			GEOMETRY = 3000,
			MASKING = GEOMETRY - 10,
			MASKED = GEOMETRY + 10,
		}

		public RenderQueueType queue;
		public bool applyToAllChildren;
		public Renderer[] targets;

		void Reset() {
			queue = RenderQueueType.GEOMETRY;
		}

		void Awake() {
			foreach (Renderer r in targets.Union(Children()))
				r.SetRenderQueue((int)queue);
		}

		IEnumerable<Renderer> Children() {
			return (applyToAllChildren)
				? gameObject.GetComponents<Renderer>(-1, true)
				: GeneralR.Empty<Renderer>();
		}
	}
}
