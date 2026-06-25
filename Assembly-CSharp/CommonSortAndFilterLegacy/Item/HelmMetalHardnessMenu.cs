using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004D8 RID: 1240
	public class HelmMetalHardnessMenu : HelmCommonMakeTypeMenu
	{
		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x06004255 RID: 16981 RVA: 0x00203F0F File Offset: 0x0020210F
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000773 RID: 1907
		// (get) Token: 0x06004256 RID: 16982 RVA: 0x00203F12 File Offset: 0x00202112
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 3));
			}
		}

		// Token: 0x17000774 RID: 1908
		// (get) Token: 0x06004257 RID: 16983 RVA: 0x00203F20 File Offset: 0x00202120
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}
	}
}
