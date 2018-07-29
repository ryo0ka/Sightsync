using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DriftCorrection {
	// Object pool system that is quite simple.
	// Doesn't deal with behaviors of pooled/unpooled objects.
	// So if you want them to e.g. toggle active, do it manually.
	public class Poolable : MonoBehaviour {
		[Serializable]
		public class Pool: IEnumerable<Poolable> {
			[SerializeField]
			public Poolable original;
			
			List<Poolable> pool;

			Pool() {
				pool = new List<Poolable>();
			}

			public Poolable Unpool() {
				for (int i = 0; true; i++) {
					if (i >= pool.Count) {
						Poolable obj = GameObject.Instantiate(
							original: original,
							parent: original.transform.parent);
						pool.Add(obj);
						return obj;
					} else {
						Poolable obj = pool[i];
						if (obj.Pooled) {
							obj.Pooled = false;
							return obj;
						}
					}
				}
			}

			public IEnumerator<Poolable> GetEnumerator() {
				foreach (var n in pool) yield return n;
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}

		public virtual bool Pooled { get; set; }
	}
}
