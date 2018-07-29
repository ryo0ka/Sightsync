using UnityEngine;

namespace Assets.Ryooka.Scripts.Misc {
	class Note : MonoBehaviour {
		[SerializeField]
		[TextArea(3, 20)]
		string note;
	}
}
