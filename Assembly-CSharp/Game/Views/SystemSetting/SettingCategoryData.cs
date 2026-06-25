using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200076D RID: 1901
	public class SettingCategoryData
	{
		// Token: 0x17000AC8 RID: 2760
		// (get) Token: 0x06005BF1 RID: 23537 RVA: 0x002AAB57 File Offset: 0x002A8D57
		public ESettingCategory Category { get; }

		// Token: 0x17000AC9 RID: 2761
		// (get) Token: 0x06005BF2 RID: 23538 RVA: 0x002AAB5F File Offset: 0x002A8D5F
		public SubCategoryInfo[] SubCategories { get; }

		// Token: 0x06005BF3 RID: 23539 RVA: 0x002AAB67 File Offset: 0x002A8D67
		public SettingCategoryData(ESettingCategory category, SubCategoryInfo[] subCategories)
		{
			this.Category = category;
			this.SubCategories = subCategories;
		}
	}
}
