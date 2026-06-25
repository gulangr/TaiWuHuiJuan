using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000778 RID: 1912
	[AttributeUsage(AttributeTargets.Property)]
	public class EnumSettingAttribute : SettingItemBaseAttribute
	{
		// Token: 0x17000AE5 RID: 2789
		// (get) Token: 0x06005C36 RID: 23606 RVA: 0x002AB659 File Offset: 0x002A9859
		public override SettingUIType UIType
		{
			get
			{
				return SettingUIType.Dropdown;
			}
		}

		// Token: 0x17000AE6 RID: 2790
		// (get) Token: 0x06005C37 RID: 23607 RVA: 0x002AB65C File Offset: 0x002A985C
		// (set) Token: 0x06005C38 RID: 23608 RVA: 0x002AB664 File Offset: 0x002A9864
		public LanguageKey[] Options { get; set; }

		// Token: 0x06005C39 RID: 23609 RVA: 0x002AB66D File Offset: 0x002A986D
		public EnumSettingAttribute(ESettingSubCategory subCategory, int order, LanguageKey languageKey) : base(subCategory, order, languageKey)
		{
		}
	}
}
