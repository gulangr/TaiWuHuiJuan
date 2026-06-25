using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D6E RID: 3438
	public class AccessoryWoodHardnessMenu : AccessoryCommonMakeTypeMenu
	{
		// Token: 0x17001216 RID: 4630
		// (get) Token: 0x0600A861 RID: 43105 RVA: 0x004E0CBD File Offset: 0x004DEEBD
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17001217 RID: 4631
		// (get) Token: 0x0600A862 RID: 43106 RVA: 0x004E0CC0 File Offset: 0x004DEEC0
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}

		// Token: 0x17001218 RID: 4632
		// (get) Token: 0x0600A863 RID: 43107 RVA: 0x004E0CCE File Offset: 0x004DEECE
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}
	}
}
