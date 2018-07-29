using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Ryooka.Scripts.Debugging {
	public static class DebugUtil {
		public static void LogAll(params object[] objs) {
			Debug.Log(objs.AsEnumerable().Show());
		}

		public static string Show<T>(this IEnumerable<T> self) {
			StringBuilder b = new StringBuilder();
			b.Append("[");
			foreach (var s in self)
				b.Append(s.ToString() + ", ");
			b.Append("]");
			return b.ToString();
		}

		public static T Log<T>(this T self, string format = "{0}") {
			Debug.LogFormat(format, self);
			return self;
		}

		public static T LogWarning<T>(this T self, string format = "{0}") {
			Debug.LogWarningFormat(format, self);
			return self;
		}

		public static T LogAssertion<T>(this T self, string format = "{0}") {
			Debug.LogAssertionFormat(format, self);
			return self;
		}

		public static T Assert<T>(this T self, Func<T, bool> condition, string format = "{0}") {
			if (condition(self)) {
				return self;
			} else {
				var message = string.Format(format, self);
				Debug.LogAssertion(message);
				throw new ArgumentException(message);
			}
		}

		public static T AssertNull<T>(this T self, string message = "") {
			return self.Assert(n => n != null, message);
		}

		public static bool OrAssert(this bool self, string message = "") {
			Debug.Assert(self, message);
			return self;
		}

		public static void OrAssert(params bool[] conditions) {
			Debug.Assert(conditions.All(n => n == true));
		}

		public static bool OrWarn(this bool self, string message = "") {
			if (!self) Debug.LogWarning(message);
			return self;
		}
	}
}
