using UnityEngine;

namespace Assets.Scripts.Navigation.UI {
	// Manages the position, rotation and animation of the player avatar in Miniture.
	public class MiniturePlayer : MonoBehaviour {
		// Object of the player (camera).
		// This object will be used to display the player's tranform in Miniture.
		public Transform player;

		// An object in the same coordinate system with Miniture
		// which will be used to display the player's position in the global coords.
		public Transform playerAvatar;

		// An object inside playerIndicator
		// which will be used to display the player's rotation in the global coords.
		public Transform deviceAvatar;

		[Range(0.1f, 10f)]
		public float animationSpeed;

		[Range(0f, 10f)]
		public float animationScale;

		void Update() {
			// synchronize the avatar's position with the global coords
			playerAvatar.localPosition = player.position;
			deviceAvatar.localRotation = player.rotation;

			// animate the avatar up and down
			{
				float speed = animationSpeed;

				// depends on the coords' scale,
				// so that this animation won't let the avatar jump into the sky.
				float scale = animationScale * playerAvatar.localScale.y;

				float verticalPosition = Mathf.Sin(Time.time * speed) * scale;
				Vector3 localPosition = playerAvatar.localPosition;
				localPosition.y += verticalPosition;
				playerAvatar.localPosition = localPosition;
			}
		}
	}
}
