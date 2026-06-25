using System;
using UnityEngine;

namespace PostProcessGlow
{
	// Token: 0x02000602 RID: 1538
	public interface IGlowCopy
	{
		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x06004887 RID: 18567
		Transform Transform { get; }

		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x06004888 RID: 18568
		GameObject GameObject { get; }

		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x06004889 RID: 18569
		Color GlowColor { get; }

		// Token: 0x0600488A RID: 18570
		void SetAsGlowColor();

		// Token: 0x0600488B RID: 18571
		void SetAsOriginalColor();

		// Token: 0x0600488C RID: 18572
		void Sync(IGlowSource source);
	}
}
