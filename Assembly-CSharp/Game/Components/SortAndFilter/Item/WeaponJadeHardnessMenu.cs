using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D41 RID: 3393
	public class WeaponJadeHardnessMenu : WeaponCommonMakeTypeMenu
	{
		// Token: 0x170011C1 RID: 4545
		// (get) Token: 0x0600A7C8 RID: 42952 RVA: 0x004DFBC1 File Offset: 0x004DDDC1
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170011C2 RID: 4546
		// (get) Token: 0x0600A7C9 RID: 42953 RVA: 0x004DFBC4 File Offset: 0x004DDDC4
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 2));
			}
		}

		// Token: 0x170011C3 RID: 4547
		// (get) Token: 0x0600A7CA RID: 42954 RVA: 0x004DFBD2 File Offset: 0x004DDDD2
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x0600A7CB RID: 42955 RVA: 0x004DFBD8 File Offset: 0x004DDDD8
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness;
		}
	}
}
