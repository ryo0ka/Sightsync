using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Ryooka.Scripts.Extension;

namespace Assets.Ryooka.Scripts.Misc {
	public class SetRenderQueue: MonoBehaviour {

		[SerializeField]
		protected int[] m_queues = new int[]{3000};

		protected void Awake() {
			GetComponent<Renderer>().SetRenderQueue(m_queues);
		}
	}
}
