using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000779 RID: 1913
	[AttributeUsage(AttributeTargets.Property)]
	public class ToggleGroupSettingAttribute : SettingItemBaseAttribute
	{
		// Token: 0x17000AE7 RID: 2791
		// (get) Token: 0x06005C3A RID: 23610 RVA: 0x002AB67A File Offset: 0x002A987A
		public override SettingUIType UIType
		{
			get
			{
				return SettingUIType.SwitchButton;
			}
		}

		// Token: 0x17000AE8 RID: 2792
		// (get) Token: 0x06005C3B RID: 23611 RVA: 0x002AB67D File Offset: 0x002A987D
		// (set) Token: 0x06005C3C RID: 23612 RVA: 0x002AB685 File Offset: 0x002A9885
		public LanguageKey[] Options { get; set; }

		// Token: 0x06005C3D RID: 23613 RVA: 0x002AB68E File Offset: 0x002A988E
		public ToggleGroupSettingAttribute(ESettingSubCategory subCategory, int order, LanguageKey languageKey) : base(subCategory, order, languageKey)
		{
		}
	}
}
