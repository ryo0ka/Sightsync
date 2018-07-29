﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Lightmap {
	[Serializable]
	public class RendererSnapshot {
		public Renderer source;
		public bool isTransparent;
	}
}
