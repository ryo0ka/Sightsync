using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.UI {
	[RequireComponent(typeof(Text))]
	public class TextQueue: MonoBehaviour {
		Text output {
			get { return GetComponent<Text>(); }
		}

		[SerializeField]
		int maxCharacterCount;

		[SerializeField]
		bool emphasizeFirstLine;

		[SerializeField]
		bool collapseSameLines;

		LinkedList<string> lines;

		int collapseCount;

		void Reset() {
			maxCharacterCount = 50000;
            emphasizeFirstLine = true;
		}

		void Awake() {
			lines = new LinkedList<string>();
		}

		public void AddLine(object obj) {
			AddLine(obj.ToString());
		}

		public void AddLine(string line) {
			bool collapsing = 
				collapseSameLines && 
				lines.Count > 0 && 
				lines.First.Value == line;

			if (collapsing) {
				collapseCount += 1;
			} else {
				collapseCount = 0;

				lines.AddFirst(line);
			}

			ReflectText();
		}

		void ReflectText() {
			var builder = new StringBuilder();
			var first = true;

			foreach (var line in lines) {
				var _line = line;

				if (first) {
					if (emphasizeFirstLine) {
						_line = "<b>" + line + "</b>";
					}

					if (collapseCount > 0) {
						_line += " (" + collapseCount + ")";
					}

					first = false;
				}

				builder.Append(_line);
				builder.AppendLine(); // "\n"

				if (builder.Length >= maxCharacterCount) {
					lines.RemoveLast();
					break;
				}
			}

			output.text = builder.ToString();
		}
	}
}
