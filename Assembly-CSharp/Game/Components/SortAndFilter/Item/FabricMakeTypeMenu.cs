using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DBC RID: 3516
	public class FabricMakeTypeMenu : MaterialCommonMakeTypeMenu
	{
		// Token: 0x17001279 RID: 4729
		// (get) Token: 0x0600A996 RID: 43414 RVA: 0x004E5DC3 File Offset: 0x004E3FC3
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A997 RID: 43415 RVA: 0x004E5DC8 File Offset: 0x004E3FC8
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}
	}
}
