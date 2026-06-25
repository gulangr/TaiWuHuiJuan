using System;

namespace GM
{
	// Token: 0x02000606 RID: 1542
	[AttributeUsage(AttributeTargets.Method)]
	public class GMFuncAttribute : GMMemberAttribute
	{
		// Token: 0x060048AB RID: 18603 RVA: 0x0022010D File Offset: 0x0021E30D
		public GMFuncAttribute(EGMGroup group, float width = 0.25f, int priority = 1000, string consoleName = null, GmRunMode runMode = GmRunMode.Default) : base(group, width, priority, runMode)
		{
			this.ConsoleName = consoleName;
		}

		// Token: 0x04003221 RID: 12833
		public string ConsoleName;
	}
}
