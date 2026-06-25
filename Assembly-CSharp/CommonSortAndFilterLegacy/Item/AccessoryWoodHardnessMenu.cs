using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004B1 RID: 1201
	public class AccessoryWoodHardnessMenu : AccessoryCommonMakeTypeMenu
	{
		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x060041BC RID: 16828 RVA: 0x002028ED File Offset: 0x00200AED
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x060041BD RID: 16829 RVA: 0x002028F0 File Offset: 0x00200AF0
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 1));
			}
		}

		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x060041BE RID: 16830 RVA: 0x002028FE File Offset: 0x00200AFE
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}
	}
}
