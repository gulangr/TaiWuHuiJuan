using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004BC RID: 1212
	public class BootsMetalHardnessMenu : BootsCommonMakeTypeMenu
	{
		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x060041E6 RID: 16870 RVA: 0x00202D4F File Offset: 0x00200F4F
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x060041E7 RID: 16871 RVA: 0x00202D52 File Offset: 0x00200F52
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 3));
			}
		}

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x060041E8 RID: 16872 RVA: 0x00202D60 File Offset: 0x00200F60
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}
	}
}
