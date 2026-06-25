using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudioKit
{
	// Token: 0x02000FD9 RID: 4057
	[Serializable]
	public class SoundPackage : ScriptableObject
	{
		// Token: 0x04008F96 RID: 36758
		public List<string> SeNames;

		// Token: 0x04008F97 RID: 36759
		public List<AudioClip> SeClips;
	}
}
