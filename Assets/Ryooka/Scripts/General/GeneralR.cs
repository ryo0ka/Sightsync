using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Text;

namespace Assets.Ryooka.Scripts.General {
	public static class GeneralR {
		public static T NullOrElse<T>(this T self, Func<T> def) {
			return (self == null) ? def() : self;
		}

		public static T NullOrElse<T>(this T self, T def) {
			return (self == null) ? def : self;
		}

		public static void Swap<T>(ref T m, ref T n) {
			T t = n;
			n = m;
			m = t;
		}

		public static IEnumerator StartWith(this IEnumerator self, Action onStart) {
			onStart();
			yield return null;
			while (self.MoveNext()) yield return null;
		}

		public static IEnumerator EndWith(this IEnumerator self, Action onComplete) {
			while (self.MoveNext()) yield return null;
			onComplete();
		}

		public static IEnumerator EndWith(this IEnumerator self, IEnumerator next) {
			while (self.MoveNext()) yield return null;
			while (next.MoveNext()) yield return null;
		}

		public static IEnumerator ParallelWith(this IEnumerator self, IEnumerator another) {
			while (self.MoveNext() | another.MoveNext()) yield return null;
		}

		public static IEnumerator EachWith(this IEnumerator self, Action atEach) {
			while (self.MoveNext()) {
				atEach();
				yield return null;
			}
		}

		public static IEnumerator Infinite(Action action) {
			while (true) {
				action();
				yield return null;
			}
		}

		public static IEnumerable<T> Empty<T>() {
			yield break;
		}

		public static IEnumerable<T> One<T>(T t) {
			yield return t;
		}

		public static IEnumerator EmptyCoroutine() {
			yield break;
		}

		public static IEnumerator ToCoroutine<T>(this IEnumerator<T> self, Action<T> action) {
			while (self.MoveNext()) {
				action(self.Current);
				yield return null;
			}
		}

		public static IEnumerator ToCoroutine<T>(this IEnumerable<T> self, Action<T> action) {
			return self.GetEnumerator().ToCoroutine(action);
		}

		public static IEnumerable UntilTime(float time) {
			float _time = 0;
			while (_time < time) {
				_time += Time.deltaTime;
				yield return null;
			}
		}

		public static void LogAll(params object[] objs) {
			StringBuilder b = new StringBuilder();
			bool first = true;
			foreach (object obj in objs) {
				if (!first) b.Append(",	");
				b.Append(obj.ToString());
				first = false;
			}
			Debug.Log(b.ToString());
		}

		public static T Assert<T>(this T self, Action<T> f) {
			f(self);
			return self;
		}

		public static Func<T> Const<T>(this T self) {
			return () => self;
		}

		public static Action DoNothing() {
			return () => { };
		}
	}
}
