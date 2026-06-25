using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D65 RID: 3429
	public class BootsJadeHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x17001205 RID: 4613
		// (get) Token: 0x0600A83C RID: 43068 RVA: 0x004E0571 File Offset: 0x004DE771
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17001206 RID: 4614
		// (get) Token: 0x0600A83D RID: 43069 RVA: 0x004E0574 File Offset: 0x004DE774
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 2));
			}
		}

		// Token: 0x17001207 RID: 4615
		// (get) Token: 0x0600A83E RID: 43070 RVA: 0x004E0582 File Offset: 0x004DE782
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}
	}
}
