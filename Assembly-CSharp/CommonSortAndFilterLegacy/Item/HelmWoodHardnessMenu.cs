using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004D6 RID: 1238
	public class HelmWoodHardnessMenu : HelmCommonMakeTypeMenu
	{
		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x0600424D RID: 16973 RVA: 0x00203ED5 File Offset: 0x002020D5
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x0600424E RID: 16974 RVA: 0x00203ED8 File Offset: 0x002020D8
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}

		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x0600424F RID: 16975 RVA: 0x00203EE6 File Offset: 0x002020E6
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}
	}
}
