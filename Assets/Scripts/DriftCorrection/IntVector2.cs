using System;

namespace Assets.Scripts.DriftCorrection {
	[Serializable]
	public struct IntVector2 {
		public int x, y;

		public static IntVector2 zero = new IntVector2();

		public IntVector2(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public override int GetHashCode() {
			return x + y;
		}

		public override bool Equals(object obj) {
			return (obj is IntVector2) && Equals((IntVector2)obj);
		}

		public bool Equals(IntVector2 that) {
			return this.x == that.x && this.y == that.y;
		}

		public static bool operator ==(IntVector2 v1, IntVector2 v2) {
			return v1.Equals(v2);
		}

		public static bool operator !=(IntVector2 v1, IntVector2 v2) {
			return !(v1 == v2);
		}

		public override string ToString() {
			return string.Format("({0}, {1})", x, y);
		}
	}
}
