using System;
using UnityEngine;

namespace Assets.Ryooka.Scripts.EditorExtension {
	[Serializable]
	public class UnityLayer {
		[SerializeField]
		private int m_LayerIndex = 0;

		public int LayerIndex {
			get { return m_LayerIndex; }
			set { Set(value);  }
		}

		public int Mask {
			get { return 1 << m_LayerIndex; }
		}

		private void Set(int _layerIndex) {
			if (_layerIndex > 0 && _layerIndex < 32) {
				m_LayerIndex = _layerIndex;
			}
		}
	}
}
