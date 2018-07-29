using UnityEngine;

namespace Assets.Scripts.Transition {
	[RequireComponent(typeof(EmissionFadeAnimation))]
	public class DateTransitionToken : MonoBehaviour {
		public DateTransitionManager.Token token;

		EmissionFadeAnimation anim;

		void Awake() {
			anim = GetComponent<EmissionFadeAnimation>();

			token.OnSetVisible += (visible) => {
				anim.Play(!visible);
			};

			token.Register();
		}
	}
}
