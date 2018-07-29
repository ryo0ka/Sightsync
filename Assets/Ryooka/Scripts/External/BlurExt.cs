#if false

using Assets.Ryooka.Scripts.General;
using System.Collections;
using UnityStandardAssets.ImageEffects;

namespace Assets.Ryooka.Scripts.External {
	public class BlurExt: Blur {
		public int maxIteration;
		public float maxSpread;
		public float lerpTime;
		public MathR.LerpType lerpType;

		IEnumerator Lerp(bool blurIn) {
			float itrMin = (blurIn) ? 0 : maxIteration;
			float itrMax = (blurIn) ? maxIteration : 0;
			float sprMin = (blurIn) ? 0 : maxSpread;
			float sprMax = (blurIn) ? maxSpread : 0;
			var itrEnm = MathR.LerpE(itrMin, itrMax, lerpTime, lerpType).GetEnumerator();
			var sprEnm = MathR.LerpE(sprMin, sprMax, lerpTime, lerpType).GetEnumerator();
			while (itrEnm.MoveNext() | sprEnm.MoveNext()) {
				iterations = (int)itrEnm.Current;
				blurSpread = sprEnm.Current;
				yield return null;
			}
		}

		public void LerpBlur(bool blurIn) {
			StartCoroutine(Lerp(blurIn));
		}
	}
}

#endif