using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D63 RID: 3427
	public class BootsFabricHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011FF RID: 4607
		// (get) Token: 0x0600A834 RID: 43060 RVA: 0x004E0537 File Offset: 0x004DE737
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17001200 RID: 4608
		// (get) Token: 0x0600A835 RID: 43061 RVA: 0x004E053A File Offset: 0x004DE73A
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 0));
			}
		}

		// Token: 0x17001201 RID: 4609
		// (get) Token: 0x0600A836 RID: 43062 RVA: 0x004E0548 File Offset: 0x004DE748
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}
	}
}
