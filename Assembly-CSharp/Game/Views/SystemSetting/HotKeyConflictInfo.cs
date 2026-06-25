using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200076A RID: 1898
	public struct HotKeyConflictInfo
	{
		// Token: 0x04003F6A RID: 16234
		public HotKeyCommand Command;

		// Token: 0x04003F6B RID: 16235
		public ESettingSubCategory SubCategory;

		// Token: 0x04003F6C RID: 16236
		public LanguageKey GroupLangId;

		// Token: 0x04003F6D RID: 16237
		public bool KeyConflict;

		// Token: 0x04003F6E RID: 16238
		public bool KeyCannotConfirm;

		// Token: 0x04003F6F RID: 16239
		public bool MouseKeyConflict;
	}
}
