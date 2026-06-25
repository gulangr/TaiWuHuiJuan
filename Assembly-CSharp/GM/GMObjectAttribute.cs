using System;

namespace GM
{
	// Token: 0x02000608 RID: 1544
	[AttributeUsage(AttributeTargets.Method)]
	public class GMObjectAttribute : GMMemberAttribute
	{
		// Token: 0x060048AF RID: 18607 RVA: 0x00220152 File Offset: 0x0021E352
		public GMObjectAttribute(EGMGroup group, int priority = 2000) : base(group, 0f, priority, GmRunMode.Default)
		{
		}
	}
}
