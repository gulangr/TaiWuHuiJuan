using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004B0 RID: 1200
	public class AccessoryFabricHardnessMenu : AccessoryCommonMakeTypeMenu
	{
		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x060041B8 RID: 16824 RVA: 0x002028D0 File Offset: 0x00200AD0
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x060041B9 RID: 16825 RVA: 0x002028D3 File Offset: 0x00200AD3
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 0));
			}
		}

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x060041BA RID: 16826 RVA: 0x002028E1 File Offset: 0x00200AE1
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}
	}
}
