using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200077E RID: 1918
	[AttributeUsage(AttributeTargets.Property)]
	public class HotKeySettingAttribute : SettingItemBaseAttribute
	{
		// Token: 0x17000AF0 RID: 2800
		// (get) Token: 0x06005C4C RID: 23628 RVA: 0x002AB70E File Offset: 0x002A990E
		public override SettingUIType UIType
		{
			get
			{
				return SettingUIType.HotKey;
			}
		}

		// Token: 0x17000AF1 RID: 2801
		// (get) Token: 0x06005C4D RID: 23629 RVA: 0x002AB711 File Offset: 0x002A9911
		// (set) Token: 0x06005C4E RID: 23630 RVA: 0x002AB719 File Offset: 0x002A9919
		public HotKeyCommand HotKeyCommand { get; set; }

		// Token: 0x17000AF2 RID: 2802
		// (get) Token: 0x06005C4F RID: 23631 RVA: 0x002AB722 File Offset: 0x002A9922
		// (set) Token: 0x06005C50 RID: 23632 RVA: 0x002AB72A File Offset: 0x002A992A
		public bool IsMouseKey { get; set; } = false;

		// Token: 0x06005C51 RID: 23633 RVA: 0x002AB733 File Offset: 0x002A9933
		public HotKeySettingAttribute(ESettingSubCategory subCategory, int order, LanguageKey languageKey) : base(subCategory, order, languageKey)
		{
		}
	}
}
