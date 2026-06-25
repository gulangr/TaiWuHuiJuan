using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000775 RID: 1909
	[AttributeUsage(AttributeTargets.Property)]
	public class BoolSettingAttribute : SettingItemBaseAttribute
	{
		// Token: 0x17000AD4 RID: 2772
		// (get) Token: 0x06005C14 RID: 23572 RVA: 0x002AB4CF File Offset: 0x002A96CF
		public override SettingUIType UIType
		{
			get
			{
				return SettingUIType.Toggle;
			}
		}

		// Token: 0x17000AD5 RID: 2773
		// (get) Token: 0x06005C15 RID: 23573 RVA: 0x002AB4D2 File Offset: 0x002A96D2
		// (set) Token: 0x06005C16 RID: 23574 RVA: 0x002AB4DA File Offset: 0x002A96DA
		public TipType TipType { get; set; } = TipType.SingleDesc;

		// Token: 0x17000AD6 RID: 2774
		// (get) Token: 0x06005C17 RID: 23575 RVA: 0x002AB4E3 File Offset: 0x002A96E3
		// (set) Token: 0x06005C18 RID: 23576 RVA: 0x002AB4EB File Offset: 0x002A96EB
		public LanguageKey[] ExtraTipLanguageKeys { get; set; } = null;

		// Token: 0x06005C19 RID: 23577 RVA: 0x002AB4F4 File Offset: 0x002A96F4
		public BoolSettingAttribute(ESettingSubCategory subCategory, int order, LanguageKey languageKey) : base(subCategory, order, languageKey)
		{
		}
	}
}
