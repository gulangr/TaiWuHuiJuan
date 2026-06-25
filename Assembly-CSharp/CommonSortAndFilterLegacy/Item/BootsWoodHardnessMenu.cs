using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004BA RID: 1210
	public class BootsWoodHardnessMenu : BootsCommonMakeTypeMenu
	{
		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x060041DE RID: 16862 RVA: 0x00202D15 File Offset: 0x00200F15
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x060041DF RID: 16863 RVA: 0x00202D18 File Offset: 0x00200F18
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}

		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x060041E0 RID: 16864 RVA: 0x00202D26 File Offset: 0x00200F26
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}
	}
}
