using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000777 RID: 1911
	[AttributeUsage(AttributeTargets.Property)]
	public class FloatSliderSettingAttribute : SettingItemBaseAttribute
	{
		// Token: 0x17000ADE RID: 2782
		// (get) Token: 0x06005C28 RID: 23592 RVA: 0x002AB5B0 File Offset: 0x002A97B0
		public override SettingUIType UIType
		{
			get
			{
				return SettingUIType.SliderFloat;
			}
		}

		// Token: 0x17000ADF RID: 2783
		// (get) Token: 0x06005C29 RID: 23593 RVA: 0x002AB5B3 File Offset: 0x002A97B3
		// (set) Token: 0x06005C2A RID: 23594 RVA: 0x002AB5BB File Offset: 0x002A97BB
		public float Min { get; set; } = 0f;

		// Token: 0x17000AE0 RID: 2784
		// (get) Token: 0x06005C2B RID: 23595 RVA: 0x002AB5C4 File Offset: 0x002A97C4
		// (set) Token: 0x06005C2C RID: 23596 RVA: 0x002AB5CC File Offset: 0x002A97CC
		public float Max { get; set; } = 100f;

		// Token: 0x17000AE1 RID: 2785
		// (get) Token: 0x06005C2D RID: 23597 RVA: 0x002AB5D5 File Offset: 0x002A97D5
		// (set) Token: 0x06005C2E RID: 23598 RVA: 0x002AB5DD File Offset: 0x002A97DD
		public string Format { get; set; } = "F0";

		// Token: 0x17000AE2 RID: 2786
		// (get) Token: 0x06005C2F RID: 23599 RVA: 0x002AB5E6 File Offset: 0x002A97E6
		// (set) Token: 0x06005C30 RID: 23600 RVA: 0x002AB5EE File Offset: 0x002A97EE
		public float[] SnapValues { get; set; }

		// Token: 0x17000AE3 RID: 2787
		// (get) Token: 0x06005C31 RID: 23601 RVA: 0x002AB5F7 File Offset: 0x002A97F7
		// (set) Token: 0x06005C32 RID: 23602 RVA: 0x002AB5FF File Offset: 0x002A97FF
		public float Step { get; set; } = 0.5f;

		// Token: 0x17000AE4 RID: 2788
		// (get) Token: 0x06005C33 RID: 23603 RVA: 0x002AB608 File Offset: 0x002A9808
		// (set) Token: 0x06005C34 RID: 23604 RVA: 0x002AB610 File Offset: 0x002A9810
		public bool ShowSliderLines { get; set; } = true;

		// Token: 0x06005C35 RID: 23605 RVA: 0x002AB619 File Offset: 0x002A9819
		public FloatSliderSettingAttribute(ESettingSubCategory subCategory, int order, LanguageKey languageKey) : base(subCategory, order, languageKey)
		{
		}
	}
}
