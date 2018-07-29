using System.Collections;
using System;
using UnityEngine;

namespace Assets.Ryooka.Scripts.General {
	[Serializable]
	public class NicerCoroutine {
		IEnumerator self;

		public NicerCoroutine() {
			Clear();
		}

		public void Clear() {
			self = GeneralR.EmptyCoroutine();
		}

		public void Flush() {
			while (self.MoveNext()) ;
			Clear();
		}

		public void Update() {
			self.MoveNext();
		}

		public void InitializeWith(IEnumerator another) {
			self = self.ParallelWith(another);
		}

		public void EveryUpdateWith(Action action) {
			self = self.ParallelWith(GeneralR.Infinite(action));
		}
	}
}
