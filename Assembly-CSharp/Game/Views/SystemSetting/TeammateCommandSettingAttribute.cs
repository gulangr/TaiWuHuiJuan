using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200077D RID: 1917
	[AttributeUsage(AttributeTargets.Property)]
	public class TeammateCommandSettingAttribute : SettingItemBaseAttribute
	{
		// Token: 0x17000AEF RID: 2799
		// (get) Token: 0x06005C4A RID: 23626 RVA: 0x002AB6FE File Offset: 0x002A98FE
		public override SettingUIType UIType
		{
			get
			{
				return SettingUIType.TeammateCommand;
			}
		}

		// Token: 0x06005C4B RID: 23627 RVA: 0x002AB701 File Offset: 0x002A9901
		public TeammateCommandSettingAttribute(ESettingSubCategory subCategory, int order, LanguageKey languageKey) : base(subCategory, order, languageKey)
		{
		}
	}
}
