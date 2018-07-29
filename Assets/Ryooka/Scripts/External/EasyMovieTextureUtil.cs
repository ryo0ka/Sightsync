#if false

using System;
using UnityEngine;
using Assets.Ryooka.Scripts.Extension;
using System.Collections;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.External {
	public static class EasyMovieTextureUtil {
		public static void SetTime(this Animation self, MediaPlayerCtrl player) {
			self.SetTime((float)player.GetSeekPosition() / 1000);
		}

		// in second format.
		public static float GetCurrentTime(this MediaPlayerCtrl self) {
			return (float)self.GetSeekPosition() / 1000;
		}

		// in second format.
		public static void SeekToTime(this MediaPlayerCtrl self, float time) {
			self.SeekTo((int)time * 1000);
		}

		// note: make sure to have them ready beforehand.
		public static void SynchronizeWith(this MediaPlayerCtrl self, MediaPlayerCtrl target) {
			self.SeekTo(target.GetSeekPosition());
		}

		public static bool IsPlaying(this MediaPlayerCtrl self) {
			return self.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING;
		}

		public static bool IsLoaded(this MediaPlayerCtrl self) {
			switch (self.GetCurrentState()) {
				case MediaPlayerCtrl.MEDIAPLAYER_STATE.READY: return true;
				case MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED: return true;
				case MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING: return true;
				case MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED: return true;
				default: return false;
			}
		}

		public static void ForEachRenderer(this MediaPlayerCtrl self, Action<Renderer> f) {
			foreach (var o in self.m_TargetMaterial) f(o.GetComponent<Renderer>());
		}

		public static void DrawAsBackground(this MediaPlayerCtrl self) {
			self.ForEachRenderer(r => r.RenderAsBackground());
		}

		public static void SetAlpha(this MediaPlayerCtrl self, float value) {
			self.ForEachRenderer(r => r.SetAlpha(value));
		}

		public static IEnumerable Fade(this MediaPlayerCtrl self, float start, float end, float time, MathR.LerpType type) {
			foreach (var value in MathR.LerpE(start, end, time, type)) {
				self.ForEachRenderer(r => r.SetAlpha(value));
				yield return null;
			}
		}

		public static IEnumerable Fade(this MediaPlayerCtrl self, bool fadeIn, float time, MathR.LerpType type) {
			float start = (fadeIn) ? 0 : 1;
			float end   = (fadeIn) ? 1 : 0;
			return self.Fade(start, end, time, type);
		}

		public static IEnumerable FadeIn(this MediaPlayerCtrl self, float time, MathR.LerpType type) {
			return self.Fade(true, time, type);
		}

		public static IEnumerable FadeOut(this MediaPlayerCtrl self, float time, MathR.LerpType type) {
			return self.Fade(false, time, type);
		}

		public static IEnumerable FadeInto(this MediaPlayerCtrl self, MediaPlayerCtrl target, float time) {
			var fOt = self.FadeOut(time, MathR.LerpType.LINEAR).GetEnumerator();
			var fIn = target.FadeIn(time, MathR.LerpType.LINEAR).GetEnumerator();
			while (fOt.MoveNext() | fIn.MoveNext()) yield return null;
		}
	}
}

#endif