using UnityEngine;

namespace Assets.Ryooka.Scripts.Misc {
	public class SetResolution : MonoBehaviour{
		public int width;
		public int height;

		void Awake() {
			Screen.SetResolution(width, height, true);
		}
	}
}
