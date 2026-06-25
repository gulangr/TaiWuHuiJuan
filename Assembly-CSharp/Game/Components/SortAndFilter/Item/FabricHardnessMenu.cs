using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DBD RID: 3517
	public class FabricHardnessMenu : MaterialCommonHardnessMenu
	{
		// Token: 0x1700127A RID: 4730
		// (get) Token: 0x0600A999 RID: 43417 RVA: 0x004E5DED File Offset: 0x004E3FED
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A99A RID: 43418 RVA: 0x004E5DF0 File Offset: 0x004E3FF0
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness;
		}

		// Token: 0x0600A99B RID: 43419 RVA: 0x004E5E0C File Offset: 0x004E400C
		protected override IEnumerable<EMaterialFilterHardness> GenerateMaterialHardnessList()
		{
			yield return EMaterialFilterHardness.Fur;
			yield return EMaterialFilterHardness.Woven;
			yield break;
		}
	}
}
