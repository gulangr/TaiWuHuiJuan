using System;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004EF RID: 1263
	public class WeaponHerbMakeTypeMenu : WeaponCommonMakeTypeMenu
	{
		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x060042AE RID: 17070 RVA: 0x00204B0B File Offset: 0x00202D0B
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x060042AF RID: 17071 RVA: 0x00204B0E File Offset: 0x00202D0E
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 4));
			}
		}

		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x060042B0 RID: 17072 RVA: 0x00204B1C File Offset: 0x00202D1C
		protected override sbyte MyResourceType
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x060042B1 RID: 17073 RVA: 0x00204B20 File Offset: 0x00202D20
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}
	}
}
