using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Extension {
	public static class ColliderUtil {
		//Returns already assigned collider if any.
		//Returns newly assigned MeshCollider if MeshFilter is assigned.
		//Returns null otherwise.
		public static Collider GetOrAddCollider(this GameObject self) {
			var collider = self.GetComponent<Collider>();
			if (collider != null) return collider;
			var meshFilter = self.GetComponent<MeshFilter>();
			if (meshFilter == null) return null;
			var meshCollider = self.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = meshFilter.sharedMesh;
			return meshCollider;
		}

		public static bool IsLookedAtBy(this Collider self, Camera camera) {
			Ray cameraRay = new Ray(
				origin:    camera.transform.position,
				direction: camera.transform.forward);
			return self.bounds.IntersectRay(cameraRay);
		}
	}
}
