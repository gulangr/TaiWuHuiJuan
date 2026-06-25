using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004C3 RID: 1219
	public class BracersWoodHardnessMenu : BracersCommonMakeTypeMenu
	{
		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x06004200 RID: 16896 RVA: 0x0020313D File Offset: 0x0020133D
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x06004201 RID: 16897 RVA: 0x00203140 File Offset: 0x00201340
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}

		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x06004202 RID: 16898 RVA: 0x0020314E File Offset: 0x0020134E
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}
	}
}
