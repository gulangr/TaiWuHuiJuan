using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D70 RID: 3440
	public class AccessoryMetalHardnessMenu : AccessoryCommonMakeTypeMenu
	{
		// Token: 0x1700121C RID: 4636
		// (get) Token: 0x0600A869 RID: 43113 RVA: 0x004E0CF7 File Offset: 0x004DEEF7
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x1700121D RID: 4637
		// (get) Token: 0x0600A86A RID: 43114 RVA: 0x004E0CFA File Offset: 0x004DEEFA
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 3));
			}
		}

		// Token: 0x1700121E RID: 4638
		// (get) Token: 0x0600A86B RID: 43115 RVA: 0x004E0D08 File Offset: 0x004DEF08
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}
	}
}
