using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200077A RID: 1914
	[AttributeUsage(AttributeTargets.Property)]
	public class MultiToggleGroupSettingAttribute : SettingItemBaseAttribute
	{
		// Token: 0x17000AE9 RID: 2793
		// (get) Token: 0x06005C3E RID: 23614 RVA: 0x002AB69B File Offset: 0x002A989B
		public override SettingUIType UIType
		{
			get
			{
				return SettingUIType.MultiToggleGroup;
			}
		}

		// Token: 0x17000AEA RID: 2794
		// (get) Token: 0x06005C3F RID: 23615 RVA: 0x002AB69E File Offset: 0x002A989E
		// (set) Token: 0x06005C40 RID: 23616 RVA: 0x002AB6A6 File Offset: 0x002A98A6
		public LanguageKey[] Options { get; set; }

		// Token: 0x17000AEB RID: 2795
		// (get) Token: 0x06005C41 RID: 23617 RVA: 0x002AB6AF File Offset: 0x002A98AF
		// (set) Token: 0x06005C42 RID: 23618 RVA: 0x002AB6B7 File Offset: 0x002A98B7
		public LanguageKey[] ExtraTipLanguageKeys { get; set; }

		// Token: 0x06005C43 RID: 23619 RVA: 0x002AB6C0 File Offset: 0x002A98C0
		public MultiToggleGroupSettingAttribute(ESettingSubCategory subCategory, int order, LanguageKey languageKey) : base(subCategory, order, languageKey)
		{
		}
	}
}
