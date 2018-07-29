using UnityEngine;
using System.Collections.Generic;

namespace Assets.Ryooka.Scripts.General {
	static public class MathR {
		public enum LerpType { LINEAR, EASE_OUT, SMOOTH_STEP, SMOOTHER_STEP }

		//https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
		static public float LerpTime(float time, LerpType type) {
			switch (type) {
				case LerpType.EASE_OUT: return Mathf.Sin(time * Mathf.PI * 0.5f);
				case LerpType.SMOOTH_STEP: return time = time * time * (3f - 2f * time);
				case LerpType.SMOOTHER_STEP: return time * time * time * (time * (6f * time - 15f) + 10f);
				default: return time;
			}
		}

		// Lerp but doesn't cut off outbound value
		static public float Lerp(float min, float max, float time, LerpType type) {
			return Map(LerpTime(time, type), 0f, 1f, min, max);
		}
		
		static public IEnumerable<float> LerpE(float min, float max, float time, LerpType type) {
			float currentTime = 0;
			while (currentTime <= time) {
				currentTime += Time.deltaTime;
				yield return Lerp(min, max, currentTime / time, type);
			}
		}

		static public IEnumerable<float> LerpE(float min, float max, float time) {
			return LerpE(min, max, time, LerpType.LINEAR);
		}

		static public float Map(float valA, float minA, float maxA, float minB, float maxB) {
			return (valA - minA) / (maxA - minA) * (maxB - minB) + minB;
		}

		static public float Normalize(float val, float min, float max) {
			return Map(val, min, max, 0, 1);
		}

		public static float ZeroIllegal(this float n) {
			return (float.IsNaN(n) || float.IsInfinity(n)) ? 0 : n;
		}

		public static float Smooth(float current, float previous, float amount) {
			amount = Mathf.Max(1, amount + 1);
			return previous + (current - previous) / amount;
		}

		public static bool OutOfBounds(this float value, float min, float max) {
			return value < min || max < value;
		}

		public static bool InBounds(this float value, float min, float max) {
			return !value.OutOfBounds(min, max);
		}
	}
}
