using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.Misc {
	[Serializable]
	public class TransformSerializer {
		[Serializable]
		class TransformData {
			[SerializeField]
			public string name;

			[SerializeField]
			public Vector3 position;

			[SerializeField]
			public Quaternion rotation;
		}

		[SerializeField]
		List<Transform> targets;

		// When an deserialied transform is not found in targets,
		// this function will be called to get an alternative target.
		// The returned target transform will be then added to targets.
		// Null should be returned if a given name should not be applied to any objects.
		public event Func<string, Transform> alternativeTarget;

		TransformSerializer() { }

		public IEnumerable<Transform> AllTargets() {
			return targets.AsEnumerable();
		}

		public string[] Serialize() {
			var targetsData = targets.Select(t => new TransformData {
				name     = t.name,
				position = t.position,
				rotation = t.rotation,
			});

			return SimpleSerializer.Serialize(targetsData);
		}

		public void Deserialize(string[] lines) {
			foreach (var data in SimpleSerializer.Deserialize<TransformData>(lines)) {
				var target = FindNamedTarget(data.name);
				if (target != null) {
					target.position = data.position;
					target.rotation = data.rotation;
				} else {
					Debug.LogWarning("No transform found: " + data.name);
				}
			}
		}

		Transform FindNamedTarget(string name) {
			var target = targets.FirstOrDefault(t => t.name == name);
			if (target == null) {
				target = alternativeTarget(name);
				if (target != null) {
					targets.Add(target);
				}
			}
			return target;
		}
	}
}
