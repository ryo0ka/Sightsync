using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Ryooka.Scripts.General {
	public static class QuaternionR {
		public static Quaternion Average(IEnumerable<Quaternion> qs) {
			int length = qs.Count();
			if (length == 0) return Quaternion.identity;
			if (length == 1) return qs.ElementAt(0);
			Vector4 cumulative = Vector4.zero;
			Quaternion init = qs.ElementAt(0);
			return qs.Where((_, i) => i > 0).Aggregate((_, q) =>
				Math3d.AverageQuaternion(ref cumulative, q, init, length));
		}

		public static Quaternion Zip(Quaternion one, Quaternion two, Func<float, float, float> f) {
			return new Quaternion(f(one.x, two.x), f(one.y, two.y), f(one.z, two.z), f(one.w, two.w));
		}

		public static Quaternion Map(this Quaternion self, Func<float, float> f) {
			return new Quaternion(f(self.x), f(self.y), f(self.z), f(self.w));
		}

		public static Quaternion Smooth(this Quaternion self, Quaternion previous, float amount) {
			Quaternion pq = previous;
			Quaternion cq = self;
			Quaternion rq = Quaternion.identity;
			rq.x = MathR.Smooth(cq.x, pq.x, amount);
			rq.y = MathR.Smooth(cq.y, pq.y, amount);
			rq.z = MathR.Smooth(cq.z, pq.z, amount);
			rq.w = MathR.Smooth(cq.w, pq.w, amount);
			return rq;
		}
	}
}
