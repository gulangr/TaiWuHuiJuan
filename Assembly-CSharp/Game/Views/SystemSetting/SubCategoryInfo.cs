using System;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200076E RID: 1902
	public class SubCategoryInfo
	{
		// Token: 0x17000ACA RID: 2762
		// (get) Token: 0x06005BF4 RID: 23540 RVA: 0x002AAB7F File Offset: 0x002A8D7F
		public ESettingSubCategory SubCategory { get; }

		// Token: 0x17000ACB RID: 2763
		// (get) Token: 0x06005BF5 RID: 23541 RVA: 0x002AAB87 File Offset: 0x002A8D87
		public LanguageKey Title { get; }

		// Token: 0x06005BF6 RID: 23542 RVA: 0x002AAB8F File Offset: 0x002A8D8F
		public SubCategoryInfo(ESettingSubCategory subCategory, LanguageKey title)
		{
			this.SubCategory = subCategory;
			this.Title = title;
		}
	}
}
