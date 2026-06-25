using System;

namespace GM
{
	// Token: 0x0200060E RID: 1550
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class GMFuncArgAttribute : Attribute
	{
		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x060048B5 RID: 18613 RVA: 0x002201AB File Offset: 0x0021E3AB
		public int Index { get; }

		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x060048B6 RID: 18614 RVA: 0x002201B3 File Offset: 0x0021E3B3
		public EWidgetType WidgetType { get; }

		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x060048B7 RID: 18615 RVA: 0x002201BB File Offset: 0x0021E3BB
		public float Width { get; }

		// Token: 0x060048B8 RID: 18616 RVA: 0x002201C3 File Offset: 0x0021E3C3
		public GMFuncArgAttribute(int index, EWidgetType widgetType, float width = 0.1f)
		{
			this.Index = index;
			this.WidgetType = widgetType;
			this.Width = width;
		}
	}
}
