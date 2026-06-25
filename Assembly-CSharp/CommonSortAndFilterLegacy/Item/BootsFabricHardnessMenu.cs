using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004B9 RID: 1209
	public class BootsFabricHardnessMenu : BootsCommonMakeTypeMenu
	{
		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x060041DA RID: 16858 RVA: 0x00202CF8 File Offset: 0x00200EF8
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x060041DB RID: 16859 RVA: 0x00202CFB File Offset: 0x00200EFB
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 0));
			}
		}

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x060041DC RID: 16860 RVA: 0x00202D09 File Offset: 0x00200F09
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}
	}
}
