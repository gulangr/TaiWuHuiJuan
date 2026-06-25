using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DC0 RID: 3520
	public class WoodHardnessMenu : MaterialCommonHardnessMenu
	{
		// Token: 0x1700127E RID: 4734
		// (get) Token: 0x0600A9A5 RID: 43429 RVA: 0x004E5E99 File Offset: 0x004E4099
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A9A6 RID: 43430 RVA: 0x004E5E9C File Offset: 0x004E409C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness;
		}

		// Token: 0x0600A9A7 RID: 43431 RVA: 0x004E5EB8 File Offset: 0x004E40B8
		protected override IEnumerable<EMaterialFilterHardness> GenerateMaterialHardnessList()
		{
			yield return EMaterialFilterHardness.Wooden;
			yield return EMaterialFilterHardness.Rattan;
			yield break;
		}
	}
}
