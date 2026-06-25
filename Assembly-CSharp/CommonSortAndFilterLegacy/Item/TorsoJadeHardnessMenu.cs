using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004E1 RID: 1249
	public class TorsoJadeHardnessMenu : TorsoCommonMakeTypeMenu
	{
		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x06004278 RID: 17016 RVA: 0x0020435A File Offset: 0x0020255A
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x06004279 RID: 17017 RVA: 0x0020435D File Offset: 0x0020255D
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 2));
			}
		}

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x0600427A RID: 17018 RVA: 0x0020436B File Offset: 0x0020256B
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}
	}
}
