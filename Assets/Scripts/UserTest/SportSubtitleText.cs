using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace Assets.Scripts.UserTest {
	public class SportSubtitleText: MonoBehaviour {
		[TextArea]
		[SerializeField]
		string text;

		[SerializeField]
		Text display;

		[SerializeField]
		float timePerChar;

		[SerializeField]
		int maxLineCount;

		[Range(0, 37)]
		[SerializeField]
		int lengthPerLine;

		[Range(0, 1)]
		[SerializeField]
		float progress;

		// Unity function
		void Reset() {
			display = GetComponent<Text>();
			timePerChar = 0.1f;
			maxLineCount = 2;
			lengthPerLine = 30;
			progress = 0;
		}

		// Unity function
		void OnValidate() {
			Display(progress);
		}

		void Display(float progress) {
			this.progress = progress;

			var length = (int) (text.Length * progress);
			var lineCount = length / lengthPerLine + 1;
			var overflowLineCount = Mathf.Max(lineCount - maxLineCount, 0);
			var overflowLength = overflowLineCount * lengthPerLine;
			var subbedLength = length - overflowLength;
			var subbedText = text.Substring(overflowLength, subbedLength);

			var output = new StringBuilder();
			for (int i = 0; i < subbedText.Length; ++i) {
				if (i > 0 && i % lengthPerLine == 0) {
					output.Append('\n');
				}
				output.Append(subbedText[i]);
			}
			display.text = output.ToString();
		}

		IEnumerator _StartSubtitle() {
			for (int i = 0; i < text.Length; ++i) {
				var progress = (float) i / text.Length;
				Display(progress);
				yield return new WaitForSeconds(timePerChar);
			}
		}

		public void StartSubtitle() {
			if (!Application.isPlaying) return;

			StopAllCoroutines();
			StartCoroutine(_StartSubtitle());
		}
	}
}
