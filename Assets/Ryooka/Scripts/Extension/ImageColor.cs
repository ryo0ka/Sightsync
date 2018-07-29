using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ryooka.Scripts.Extension {
    [RequireComponent(typeof(Image))]
    public class ImageColor : MonoBehaviour {
        private Color color {
            get { return GetComponent<Image>().color; }
            set { GetComponent<Image>().color = value; }
        }

        public void SetAlpha(float value) {
            var clr = color;
            clr.a = value;
            color = clr;
        }
    }
}
