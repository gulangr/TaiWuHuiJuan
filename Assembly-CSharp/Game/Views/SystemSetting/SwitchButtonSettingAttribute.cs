using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200077C RID: 1916
	[AttributeUsage(AttributeTargets.Property)]
	public class SwitchButtonSettingAttribute : SettingItemBaseAttribute
	{
		// Token: 0x17000AED RID: 2797
		// (get) Token: 0x06005C46 RID: 23622 RVA: 0x002AB6DD File Offset: 0x002A98DD
		public override SettingUIType UIType
		{
			get
			{
				return SettingUIType.SwitchButton;
			}
		}

		// Token: 0x17000AEE RID: 2798
		// (get) Token: 0x06005C47 RID: 23623 RVA: 0x002AB6E0 File Offset: 0x002A98E0
		// (set) Token: 0x06005C48 RID: 23624 RVA: 0x002AB6E8 File Offset: 0x002A98E8
		public LanguageKey[] Options { get; set; }

		// Token: 0x06005C49 RID: 23625 RVA: 0x002AB6F1 File Offset: 0x002A98F1
		public SwitchButtonSettingAttribute(ESettingSubCategory subCategory, int order, LanguageKey languageKey) : base(subCategory, order, languageKey)
		{
		}
	}
}
