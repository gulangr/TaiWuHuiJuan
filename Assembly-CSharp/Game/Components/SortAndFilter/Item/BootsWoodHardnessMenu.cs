using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D64 RID: 3428
	public class BootsWoodHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x17001202 RID: 4610
		// (get) Token: 0x0600A838 RID: 43064 RVA: 0x004E0554 File Offset: 0x004DE754
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17001203 RID: 4611
		// (get) Token: 0x0600A839 RID: 43065 RVA: 0x004E0557 File Offset: 0x004DE757
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}

		// Token: 0x17001204 RID: 4612
		// (get) Token: 0x0600A83A RID: 43066 RVA: 0x004E0565 File Offset: 0x004DE765
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}
	}
}
