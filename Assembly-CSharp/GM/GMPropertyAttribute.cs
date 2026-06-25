using System;

namespace GM
{
	// Token: 0x02000607 RID: 1543
	[AttributeUsage(AttributeTargets.Property)]
	public class GMPropertyAttribute : GMMemberAttribute
	{
		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x060048AC RID: 18604 RVA: 0x00220124 File Offset: 0x0021E324
		public float ValueWidth { get; }

		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x060048AD RID: 18605 RVA: 0x0022012C File Offset: 0x0021E32C
		public EWidgetType WidgetType { get; }

		// Token: 0x060048AE RID: 18606 RVA: 0x00220134 File Offset: 0x0021E334
		public GMPropertyAttribute(EGMGroup group, float width = 0.25f, float valueWidth = 0.25f, int priority = 0, EWidgetType widgetType = EWidgetType.Auto) : base(group, width, priority, GmRunMode.Default)
		{
			this.ValueWidth = valueWidth;
			this.WidgetType = widgetType;
		}
	}
}
