using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D3F RID: 3391
	public class WeaponFabricHardnessMenu : WeaponCommonMakeTypeMenu
	{
		// Token: 0x170011BB RID: 4539
		// (get) Token: 0x0600A7BE RID: 42942 RVA: 0x004DFB4C File Offset: 0x004DDD4C
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170011BC RID: 4540
		// (get) Token: 0x0600A7BF RID: 42943 RVA: 0x004DFB4F File Offset: 0x004DDD4F
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 0));
			}
		}

		// Token: 0x170011BD RID: 4541
		// (get) Token: 0x0600A7C0 RID: 42944 RVA: 0x004DFB5D File Offset: 0x004DDD5D
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x0600A7C1 RID: 42945 RVA: 0x004DFB60 File Offset: 0x004DDD60
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness;
		}
	}
}
