#if false

using System.Collections;
using UnityEngine;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Ryooka.Scripts.External {
	public class SwapVideo : MonoBehaviour {
		public new Camera camera;
		public MediaPlayerCtrl player1;
		public MediaPlayerCtrl player2;
		public float swapTime;

		bool p2active;
		bool swapping;

		MediaPlayerCtrl front {
			get { return (!p2active) ? player1 : player2; }
		}

		MediaPlayerCtrl back {
			get { return (p2active) ? player1 : player2; }
		}

		public bool isPlaying() {
			return front.IsPlaying();
		}

		public void Play() {
			front.Play();
		}

		public void Pause() {
			front.Pause();
		}

		//Registeres the given video to the inactive player.
		public void PrepareNextClip(string path) {
			back.Load(path); //TODO callback on complete
		}

		//Synchronizes the next clip's seek to the current one's.
		public void SynchronizeClip() {
			back.SynchronizeWith(front);
		}
		
		IEnumerator SwapE(bool sync) {
			while (swapping) yield return null; //waits until ongoing swap finishes.
			swapping = true;
			if (front.IsPlaying()) back.Play();
			if (sync) SynchronizeClip();
			foreach (var _ in front.FadeInto(back, swapTime)) yield return _;
			p2active = !p2active; //now "back" pointer changes.
			back.Pause();
			swapping = false;
		}

		//Gradually fades the current player into another.
		public void SwapClip(bool sync) {
			StartCoroutine(SwapE(sync));
		}
		
		public void SwapClip() {
			StartCoroutine(SwapE(false));
		}
	}
}

#endif