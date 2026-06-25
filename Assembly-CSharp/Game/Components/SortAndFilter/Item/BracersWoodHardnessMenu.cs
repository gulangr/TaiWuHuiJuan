using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D5C RID: 3420
	public class BracersWoodHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011F3 RID: 4595
		// (get) Token: 0x0600A821 RID: 43041 RVA: 0x004E0488 File Offset: 0x004DE688
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170011F4 RID: 4596
		// (get) Token: 0x0600A822 RID: 43042 RVA: 0x004E048B File Offset: 0x004DE68B
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}

		// Token: 0x170011F5 RID: 4597
		// (get) Token: 0x0600A823 RID: 43043 RVA: 0x004E0499 File Offset: 0x004DE699
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}
	}
}
