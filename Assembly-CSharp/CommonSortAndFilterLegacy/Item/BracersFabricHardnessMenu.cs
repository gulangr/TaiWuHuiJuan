using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004C2 RID: 1218
	public class BracersFabricHardnessMenu : BracersCommonMakeTypeMenu
	{
		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x060041FC RID: 16892 RVA: 0x00203120 File Offset: 0x00201320
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x060041FD RID: 16893 RVA: 0x00203123 File Offset: 0x00201323
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 0));
			}
		}

		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x060041FE RID: 16894 RVA: 0x00203131 File Offset: 0x00201331
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}
	}
}
