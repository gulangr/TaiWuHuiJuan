using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004C4 RID: 1220
	public class BracersJadeHardnessMenu : BracersCommonMakeTypeMenu
	{
		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x06004204 RID: 16900 RVA: 0x0020315A File Offset: 0x0020135A
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700074F RID: 1871
		// (get) Token: 0x06004205 RID: 16901 RVA: 0x0020315D File Offset: 0x0020135D
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 2));
			}
		}

		// Token: 0x17000750 RID: 1872
		// (get) Token: 0x06004206 RID: 16902 RVA: 0x0020316B File Offset: 0x0020136B
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}
	}
}
