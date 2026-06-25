using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D42 RID: 3394
	public class WeaponMetalHardnessMenu : WeaponCommonMakeTypeMenu
	{
		// Token: 0x170011C4 RID: 4548
		// (get) Token: 0x0600A7CD RID: 42957 RVA: 0x004DFBFD File Offset: 0x004DDDFD
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x170011C5 RID: 4549
		// (get) Token: 0x0600A7CE RID: 42958 RVA: 0x004DFC00 File Offset: 0x004DDE00
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 3));
			}
		}

		// Token: 0x170011C6 RID: 4550
		// (get) Token: 0x0600A7CF RID: 42959 RVA: 0x004DFC0E File Offset: 0x004DDE0E
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A7D0 RID: 42960 RVA: 0x004DFC14 File Offset: 0x004DDE14
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness;
		}
	}
}
