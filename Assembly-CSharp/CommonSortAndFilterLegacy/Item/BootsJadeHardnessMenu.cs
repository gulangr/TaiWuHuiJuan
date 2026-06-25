using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004BB RID: 1211
	public class BootsJadeHardnessMenu : BootsCommonMakeTypeMenu
	{
		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x060041E2 RID: 16866 RVA: 0x00202D32 File Offset: 0x00200F32
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x060041E3 RID: 16867 RVA: 0x00202D35 File Offset: 0x00200F35
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 2));
			}
		}

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x060041E4 RID: 16868 RVA: 0x00202D43 File Offset: 0x00200F43
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}
	}
}
