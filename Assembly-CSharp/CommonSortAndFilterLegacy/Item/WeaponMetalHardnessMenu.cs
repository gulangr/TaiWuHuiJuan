using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004EE RID: 1262
	public class WeaponMetalHardnessMenu : WeaponCommonMakeTypeMenu
	{
		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x060042AA RID: 17066 RVA: 0x00204AEE File Offset: 0x00202CEE
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x060042AB RID: 17067 RVA: 0x00204AF1 File Offset: 0x00202CF1
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 3));
			}
		}

		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x060042AC RID: 17068 RVA: 0x00204AFF File Offset: 0x00202CFF
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}
	}
}
