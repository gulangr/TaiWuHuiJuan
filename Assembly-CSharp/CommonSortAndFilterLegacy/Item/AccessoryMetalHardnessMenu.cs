using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004B3 RID: 1203
	public class AccessoryMetalHardnessMenu : AccessoryCommonMakeTypeMenu
	{
		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x060041C4 RID: 16836 RVA: 0x00202927 File Offset: 0x00200B27
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x060041C5 RID: 16837 RVA: 0x0020292A File Offset: 0x00200B2A
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 3));
			}
		}

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x060041C6 RID: 16838 RVA: 0x00202938 File Offset: 0x00200B38
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}
	}
}
