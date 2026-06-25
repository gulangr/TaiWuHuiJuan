using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D43 RID: 3395
	public class WeaponHerbMakeTypeMenu : WeaponCommonMakeTypeMenu
	{
		// Token: 0x170011C7 RID: 4551
		// (get) Token: 0x0600A7D2 RID: 42962 RVA: 0x004DFC39 File Offset: 0x004DDE39
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x170011C8 RID: 4552
		// (get) Token: 0x0600A7D3 RID: 42963 RVA: 0x004DFC3C File Offset: 0x004DDE3C
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 4));
			}
		}

		// Token: 0x170011C9 RID: 4553
		// (get) Token: 0x0600A7D4 RID: 42964 RVA: 0x004DFC4A File Offset: 0x004DDE4A
		protected override sbyte MyResourceType
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x0600A7D5 RID: 42965 RVA: 0x004DFC50 File Offset: 0x004DDE50
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}
	}
}
