using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D40 RID: 3392
	public class WeaponWoodHardnessMenu : WeaponCommonMakeTypeMenu
	{
		// Token: 0x170011BE RID: 4542
		// (get) Token: 0x0600A7C3 RID: 42947 RVA: 0x004DFB85 File Offset: 0x004DDD85
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170011BF RID: 4543
		// (get) Token: 0x0600A7C4 RID: 42948 RVA: 0x004DFB88 File Offset: 0x004DDD88
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 1));
			}
		}

		// Token: 0x170011C0 RID: 4544
		// (get) Token: 0x0600A7C5 RID: 42949 RVA: 0x004DFB96 File Offset: 0x004DDD96
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A7C6 RID: 42950 RVA: 0x004DFB9C File Offset: 0x004DDD9C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness;
		}
	}
}
