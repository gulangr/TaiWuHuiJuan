using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004C5 RID: 1221
	public class BracersMetalHardnessMenu : BracersCommonMakeTypeMenu
	{
		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06004208 RID: 16904 RVA: 0x00203177 File Offset: 0x00201377
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x06004209 RID: 16905 RVA: 0x0020317A File Offset: 0x0020137A
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 3));
			}
		}

		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x0600420A RID: 16906 RVA: 0x00203188 File Offset: 0x00201388
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}
	}
}
