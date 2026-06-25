using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000774 RID: 1908
	[AttributeUsage(AttributeTargets.Property)]
	public abstract class SettingItemBaseAttribute : Attribute
	{
		// Token: 0x17000ACD RID: 2765
		// (get) Token: 0x06005C09 RID: 23561 RVA: 0x002AB450 File Offset: 0x002A9650
		public ESettingSubCategory SubCategory { get; }

		// Token: 0x17000ACE RID: 2766
		// (get) Token: 0x06005C0A RID: 23562 RVA: 0x002AB458 File Offset: 0x002A9658
		public int Order { get; }

		// Token: 0x17000ACF RID: 2767
		// (get) Token: 0x06005C0B RID: 23563 RVA: 0x002AB460 File Offset: 0x002A9660
		public LanguageKey LanguageKey { get; }

		// Token: 0x17000AD0 RID: 2768
		// (get) Token: 0x06005C0C RID: 23564 RVA: 0x002AB468 File Offset: 0x002A9668
		// (set) Token: 0x06005C0D RID: 23565 RVA: 0x002AB470 File Offset: 0x002A9670
		public LanguageKey TipLanguageKey { get; set; } = LanguageKey.Invalid;

		// Token: 0x17000AD1 RID: 2769
		// (get) Token: 0x06005C0E RID: 23566 RVA: 0x002AB479 File Offset: 0x002A9679
		// (set) Token: 0x06005C0F RID: 23567 RVA: 0x002AB481 File Offset: 0x002A9681
		public ESettingKey DependsOn { get; set; } = ESettingKey.None;

		// Token: 0x17000AD2 RID: 2770
		// (get) Token: 0x06005C10 RID: 23568 RVA: 0x002AB48A File Offset: 0x002A968A
		// (set) Token: 0x06005C11 RID: 23569 RVA: 0x002AB492 File Offset: 0x002A9692
		public ESettingKey Key { get; set; } = ESettingKey.None;

		// Token: 0x17000AD3 RID: 2771
		// (get) Token: 0x06005C12 RID: 23570
		public abstract SettingUIType UIType { get; }

		// Token: 0x06005C13 RID: 23571 RVA: 0x002AB49B File Offset: 0x002A969B
		protected SettingItemBaseAttribute(ESettingSubCategory subCategory, int order, LanguageKey languageKey)
		{
			this.SubCategory = subCategory;
			this.Order = order;
			this.LanguageKey = languageKey;
		}
	}
}
