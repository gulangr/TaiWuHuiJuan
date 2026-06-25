using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000776 RID: 1910
	[AttributeUsage(AttributeTargets.Property)]
	public class IntSliderSettingAttribute : SettingItemBaseAttribute
	{
		// Token: 0x17000AD7 RID: 2775
		// (get) Token: 0x06005C1A RID: 23578 RVA: 0x002AB50F File Offset: 0x002A970F
		public override SettingUIType UIType
		{
			get
			{
				return SettingUIType.SliderInt;
			}
		}

		// Token: 0x17000AD8 RID: 2776
		// (get) Token: 0x06005C1B RID: 23579 RVA: 0x002AB512 File Offset: 0x002A9712
		// (set) Token: 0x06005C1C RID: 23580 RVA: 0x002AB51A File Offset: 0x002A971A
		public int Min { get; set; } = 0;

		// Token: 0x17000AD9 RID: 2777
		// (get) Token: 0x06005C1D RID: 23581 RVA: 0x002AB523 File Offset: 0x002A9723
		// (set) Token: 0x06005C1E RID: 23582 RVA: 0x002AB52B File Offset: 0x002A972B
		public int Max { get; set; } = 100;

		// Token: 0x17000ADA RID: 2778
		// (get) Token: 0x06005C1F RID: 23583 RVA: 0x002AB534 File Offset: 0x002A9734
		// (set) Token: 0x06005C20 RID: 23584 RVA: 0x002AB53C File Offset: 0x002A973C
		public int Step { get; set; } = 1;

		// Token: 0x17000ADB RID: 2779
		// (get) Token: 0x06005C21 RID: 23585 RVA: 0x002AB545 File Offset: 0x002A9745
		// (set) Token: 0x06005C22 RID: 23586 RVA: 0x002AB54D File Offset: 0x002A974D
		public bool ShowSliderLines { get; set; } = true;

		// Token: 0x17000ADC RID: 2780
		// (get) Token: 0x06005C23 RID: 23587 RVA: 0x002AB556 File Offset: 0x002A9756
		// (set) Token: 0x06005C24 RID: 23588 RVA: 0x002AB55E File Offset: 0x002A975E
		public TipType TipType { get; set; } = TipType.SingleDesc;

		// Token: 0x17000ADD RID: 2781
		// (get) Token: 0x06005C25 RID: 23589 RVA: 0x002AB567 File Offset: 0x002A9767
		// (set) Token: 0x06005C26 RID: 23590 RVA: 0x002AB56F File Offset: 0x002A976F
		public LanguageKey[] ExtraTipLanguageKeys { get; set; } = null;

		// Token: 0x06005C27 RID: 23591 RVA: 0x002AB578 File Offset: 0x002A9778
		public IntSliderSettingAttribute(ESettingSubCategory subCategory, int order, LanguageKey languageKey) : base(subCategory, order, languageKey)
		{
		}
	}
}
