using UnityEngine;

namespace Assets.Ryooka.Scripts.UI {
	public class AudioSourceControl: MonoBehaviour {
		[SerializeField]
		bool playing;

		AudioSource source;

		void OnValidate() {
			Toggle(playing);
		}

		public void Toggle(bool play) {
			playing = play;
			if (play) Unpause();
			else Pause();
		}

		public void Play(AudioSource source) {
			if (Has(source)) {
				Unpause();
			} else {
				Stop();
				Assign(source);
				Play();
			}
		}

		void Stop() {
			if (source == null) return;
			source.Stop();
		}

		void Unpause() {
			if (source == null) return;
			source.UnPause();
		}

		void Pause() {
			if (source == null) return;
			source.Pause();
		}

		void Assign(AudioSource source) {
			this.source = source;
		}

		void Play() {
			if (source == null) return;
			source.Play();
		}

		bool Has(AudioSource source) {
			return this.source != null && this.source == source;
		}
	}
}
