using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004D5 RID: 1237
	public class HelmFabricHardnessMenu : HelmCommonMakeTypeMenu
	{
		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x06004249 RID: 16969 RVA: 0x00203EB8 File Offset: 0x002020B8
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x0600424A RID: 16970 RVA: 0x00203EBB File Offset: 0x002020BB
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 0));
			}
		}

		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x0600424B RID: 16971 RVA: 0x00203EC9 File Offset: 0x002020C9
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}
	}
}
