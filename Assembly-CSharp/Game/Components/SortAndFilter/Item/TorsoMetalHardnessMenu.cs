using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D56 RID: 3414
	public class TorsoMetalHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011EA RID: 4586
		// (get) Token: 0x0600A812 RID: 43026 RVA: 0x004E03F6 File Offset: 0x004DE5F6
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170011EB RID: 4587
		// (get) Token: 0x0600A813 RID: 43027 RVA: 0x004E03F9 File Offset: 0x004DE5F9
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 3));
			}
		}

		// Token: 0x170011EC RID: 4588
		// (get) Token: 0x0600A814 RID: 43028 RVA: 0x004E0407 File Offset: 0x004DE607
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}
	}
}
