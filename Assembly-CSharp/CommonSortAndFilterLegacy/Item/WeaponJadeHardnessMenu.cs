using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004ED RID: 1261
	public class WeaponJadeHardnessMenu : WeaponCommonMakeTypeMenu
	{
		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x060042A6 RID: 17062 RVA: 0x00204AD1 File Offset: 0x00202CD1
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x060042A7 RID: 17063 RVA: 0x00204AD4 File Offset: 0x00202CD4
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 2));
			}
		}

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x060042A8 RID: 17064 RVA: 0x00204AE2 File Offset: 0x00202CE2
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}
	}
}
