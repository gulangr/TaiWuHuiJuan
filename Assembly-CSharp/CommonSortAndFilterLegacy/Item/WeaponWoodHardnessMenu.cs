using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004EC RID: 1260
	public class WeaponWoodHardnessMenu : WeaponCommonMakeTypeMenu
	{
		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x060042A2 RID: 17058 RVA: 0x00204AB4 File Offset: 0x00202CB4
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x060042A3 RID: 17059 RVA: 0x00204AB7 File Offset: 0x00202CB7
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 1));
			}
		}

		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x060042A4 RID: 17060 RVA: 0x00204AC5 File Offset: 0x00202CC5
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}
	}
}
