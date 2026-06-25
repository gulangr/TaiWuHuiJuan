using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DBF RID: 3519
	public class WoodMakeTypeMenu : MaterialCommonMakeTypeMenu
	{
		// Token: 0x1700127D RID: 4733
		// (get) Token: 0x0600A9A2 RID: 43426 RVA: 0x004E5E6F File Offset: 0x004E406F
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A9A3 RID: 43427 RVA: 0x004E5E74 File Offset: 0x004E4074
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}
	}
}
