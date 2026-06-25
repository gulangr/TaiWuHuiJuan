using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004B2 RID: 1202
	public class AccessoryJadeHardnessMenu : AccessoryCommonMakeTypeMenu
	{
		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x060041C0 RID: 16832 RVA: 0x0020290A File Offset: 0x00200B0A
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x060041C1 RID: 16833 RVA: 0x0020290D File Offset: 0x00200B0D
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 2));
			}
		}

		// Token: 0x1700072C RID: 1836
		// (get) Token: 0x060041C2 RID: 16834 RVA: 0x0020291B File Offset: 0x00200B1B
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}
	}
}
