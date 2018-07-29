#if false

using System;
using System.Collections;
using UnityEngine;
using Crosstales.RTVoice;
using Assets.Ryooka.Scripts.General;

namespace Assets.Ryooka.Scripts.UI {
	public class RecordedVoice : MonoBehaviour {
		public AudioSource source;
		public bool recordOnStart;
		public float maxWaitSeconds;
		public string text;

		string recordedText;

		void Reset() {
			source = GetComponent<AudioSource>();
			recordedText = "";
			recordOnStart = true;
			maxWaitSeconds = 1f;
		}

		void Start() {
			if (recordOnStart) Record();
		}

		bool RecordVoice() {
			if (source == null) throw new InvalidOperationException();
			if (text == recordedText) return false;
			Speaker.Speak(
				text: text,
				source: source,
				speakImmediately: false);
			recordedText = text;
			return true;
		}

		IEnumerator RecordE() {
			AudioClip lastClip = source.clip;
			if (!RecordVoice()) yield break;

			foreach (var _ in GeneralR.UntilTime(maxWaitSeconds)) {
				if (source.clip != lastClip) break;
				yield return _;
			}
        }

		public void Record() {
			StartCoroutine(RecordE());
		}

		public void Play() {
			StartCoroutine(RecordE().EndWith(source.Play));
		}

		public void Pause() {
			source.Pause();
		}

		public void Resume() {
			source.UnPause();
		}

		public void Stop() {
			source.Stop();
		}

		public void ResetText(string text) {
			this.text = text;
			Record();
		}
    }
}

#endif