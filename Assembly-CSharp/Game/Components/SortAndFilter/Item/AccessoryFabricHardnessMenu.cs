using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D6D RID: 3437
	public class AccessoryFabricHardnessMenu : AccessoryCommonMakeTypeMenu
	{
		// Token: 0x17001213 RID: 4627
		// (get) Token: 0x0600A85D RID: 43101 RVA: 0x004E0CA0 File Offset: 0x004DEEA0
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17001214 RID: 4628
		// (get) Token: 0x0600A85E RID: 43102 RVA: 0x004E0CA3 File Offset: 0x004DEEA3
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 0));
			}
		}

		// Token: 0x17001215 RID: 4629
		// (get) Token: 0x0600A85F RID: 43103 RVA: 0x004E0CB1 File Offset: 0x004DEEB1
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}
	}
}
