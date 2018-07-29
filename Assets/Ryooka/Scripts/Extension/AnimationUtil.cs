using UnityEngine;

namespace Assets.Ryooka.Scripts.Extension {
	public static class AnimationUtil {
		public static void Set(this AnimationState self, float? speed = null, float? time = null, bool? forward = null) {
			bool lastForward = self.speed >= 0;
			float lastSpeed = Mathf.Abs(self.speed);
			float lastTime = (lastForward) ? self.time : self.length - self.time;
			bool newForward = forward.GetValueOrDefault(lastForward);
			float newSpeed = speed.GetValueOrDefault(lastSpeed);
			float newTime = time.GetValueOrDefault(lastTime);
			newTime = Mathf.Clamp(newTime, 0, self.length);
			self.speed = (newForward) ? newSpeed : -newSpeed;
			self.time = (newForward) ? newTime : self.length - newTime;
		}

		public static void SetState(this Animation self, float? speed = null, float? time = null, bool? forward = null) {
			foreach (AnimationState state in self) {
				state.Set(speed, time, forward);
			}
		}

		public static void SetTime(this Animation self, float time) {
			self.SetState(time: time);
		}
	}
}
