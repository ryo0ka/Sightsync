using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Ryooka.Scripts.General {
	public class Pairs<K, V> {
		Dictionary<K, V> pool;
		Func<K, V> instantiate;
		Func<V, bool> check;
        Action<V> delete;

		public Pairs(Func<K, V> instantiate, Func<V, bool> check, Action<V> delete) {
			pool = new Dictionary<K, V>();
			this.instantiate = instantiate;
			this.check = check;
			this.delete = delete;
		}

		public V GetInstance(K key) {
			V value;
			pool.TryGetValue(key, out value);
			return value;
		}

		public bool HasInstance(K key) {
			var value = GetInstance(key);
			return value != null && check(value);
		}

		public V GetOrInstantiate(K key) {
			V value = GetInstance(key);
			if (value != null && check(value)) {
				return value;
			} else {
				DestroyInstance(key);
				value = instantiate(key);
				pool.Add(key, value);
				return value;
			}
		}

		public void DestroyInstance(K key) {
			V value = GetInstance(key);
			if (value != null) delete(value);
			pool.Remove(key);
		}

		public void DestroyAllInstances() {
			foreach (var k in pool.Keys) DestroyInstance(k);
		}
	}
}
