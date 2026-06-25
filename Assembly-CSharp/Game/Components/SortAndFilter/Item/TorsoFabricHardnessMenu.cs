using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D53 RID: 3411
	public class TorsoFabricHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011E1 RID: 4577
		// (get) Token: 0x0600A806 RID: 43014 RVA: 0x004E039F File Offset: 0x004DE59F
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170011E2 RID: 4578
		// (get) Token: 0x0600A807 RID: 43015 RVA: 0x004E03A2 File Offset: 0x004DE5A2
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 0));
			}
		}

		// Token: 0x170011E3 RID: 4579
		// (get) Token: 0x0600A808 RID: 43016 RVA: 0x004E03B0 File Offset: 0x004DE5B0
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}
	}
}
