#if true
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Tango;

namespace Assets.Ryooka.Scripts.External {
	// Saves ADFs (avoids multiple saving threads to run)
	// Loads ADFs with specified names
	[Serializable]
	public class TangoADFManager {

		TangoADFManager() { }
	}
}
#endif