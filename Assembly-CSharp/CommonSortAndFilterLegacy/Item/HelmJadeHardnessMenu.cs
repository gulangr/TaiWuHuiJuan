using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004D7 RID: 1239
	public class HelmJadeHardnessMenu : HelmCommonMakeTypeMenu
	{
		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x06004251 RID: 16977 RVA: 0x00203EF2 File Offset: 0x002020F2
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000770 RID: 1904
		// (get) Token: 0x06004252 RID: 16978 RVA: 0x00203EF5 File Offset: 0x002020F5
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 2));
			}
		}

		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x06004253 RID: 16979 RVA: 0x00203F03 File Offset: 0x00202103
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}
	}
}
