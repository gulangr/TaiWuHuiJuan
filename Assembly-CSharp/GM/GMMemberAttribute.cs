using System;

namespace GM
{
	// Token: 0x0200060A RID: 1546
	public abstract class GMMemberAttribute : Attribute
	{
		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x060048B0 RID: 18608 RVA: 0x00220164 File Offset: 0x0021E364
		public EGMGroup Group { get; }

		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x060048B1 RID: 18609 RVA: 0x0022016C File Offset: 0x0021E36C
		public float Width { get; }

		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x060048B2 RID: 18610 RVA: 0x00220174 File Offset: 0x0021E374
		public int Priority { get; }

		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x060048B3 RID: 18611 RVA: 0x0022017C File Offset: 0x0021E37C
		public GmRunMode RunMode { get; }

		// Token: 0x060048B4 RID: 18612 RVA: 0x00220184 File Offset: 0x0021E384
		public GMMemberAttribute(EGMGroup group, float width, int priority, GmRunMode runMode = GmRunMode.Default)
		{
			this.Group = group;
			this.Width = width;
			this.Priority = priority;
			this.RunMode = runMode;
		}
	}
}
