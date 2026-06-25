using System;
using UnityEngine;

namespace PostProcessGlow
{
	// Token: 0x02000603 RID: 1539
	public interface IGlowSource
	{
		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x0600488D RID: 18573
		Color GlowColor { get; }

		// Token: 0x17000921 RID: 2337
		// (get) Token: 0x0600488E RID: 18574
		Transform Transform { get; }

		// Token: 0x0600488F RID: 18575
		IGlowCopy MakeCopy(Transform parent);
	}
}
