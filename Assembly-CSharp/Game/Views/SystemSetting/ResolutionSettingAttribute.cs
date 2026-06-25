using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200077B RID: 1915
	[AttributeUsage(AttributeTargets.Property)]
	public class ResolutionSettingAttribute : SettingItemBaseAttribute
	{
		// Token: 0x17000AEC RID: 2796
		// (get) Token: 0x06005C44 RID: 23620 RVA: 0x002AB6CD File Offset: 0x002A98CD
		public override SettingUIType UIType
		{
			get
			{
				return SettingUIType.Dropdown;
			}
		}

		// Token: 0x06005C45 RID: 23621 RVA: 0x002AB6D0 File Offset: 0x002A98D0
		public ResolutionSettingAttribute(ESettingSubCategory subCategory, int order, LanguageKey languageKey) : base(subCategory, order, languageKey)
		{
		}
	}
}
